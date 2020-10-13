using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR; 
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.PPEServer;
using Puchase_and_payables.Data; 
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class RefreshPPELPOCommand : IRequest<PaymentTermsRegRespObj>
    { 
        public class RefreshPPELPOCommandHandler : IRequestHandler<RefreshPPELPOCommand, PaymentTermsRegRespObj>
        { 
            private readonly ILoggerService _logger;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IPPEServerRequest _pperequest;
            private readonly DataContext _dataContext;
            private readonly IMapper _mapper; 
            public RefreshPPELPOCommandHandler(
                ILoggerService loggerService,
                IMapper mapper,
                IIdentityServerRequest serverRequest,
                DataContext dataContext,
                IPPEServerRequest request
                )
            {
                _mapper = mapper;
                _dataContext = dataContext;
                _logger = loggerService;
                _pperequest = request;
                _serverRequest = serverRequest;
            } 
            public async Task<PaymentTermsRegRespObj> Handle(RefreshPPELPOCommand request, CancellationToken cancellationToken)
            {
                var response = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var user = await _serverRequest.UserDataAsync();
                    if (user == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Process This User";
                        return response; 
                    }
                    var ppeclasses = await _pperequest.GetAllPPEClassificationsAsync();
                    if(ppeclasses.AssetClassifications.Count() > 0)
                    {
                        var lpos = await _dataContext.purch_plpo.ToListAsync();

                        var typePPELpos = (from a in lpos
                                           join b in _dataContext.cor_supplier on a.WinnerSupplierId equals b.SupplierId
                                           join c in _dataContext.cor_suppliertype on b.SupplierTypeId equals c.SupplierTypeId
                                           join d in ppeclasses.AssetClassifications on c.DebitGL equals d.SubGlAddition
                                           select a).ToList();

                        var typePPELposmapped = _mapper.Map<List<UpdatePPELPO>>(typePPELpos);
                        await _pperequest.CreateUpdateLPOForAdditionAsync(typePPELposmapped);
                    }
                     
                    response.Status.IsSuccessful = true;
                    response.Status.Message.FriendlyMessage = "Successful";
                    return response;
                }
                catch (Exception ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    return new PaymentTermsRegRespObj
                    {

                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Error occured!! Unable to process item",
                                MessageId = errorCode,
                                TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                            }
                        }
                    };
                    #endregion
                }
            } 
        }

        
    }
    
}