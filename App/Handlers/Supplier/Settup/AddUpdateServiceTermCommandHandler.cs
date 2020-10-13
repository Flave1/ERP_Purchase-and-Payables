using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Supplier.setup;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup
{ 

    public class AddUpdateServiceTermCommandHandler : IRequestHandler<AddUpdateServiceTermCommand, ServiceTermRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly IIdentityServerRequest _serverRequest;
        public readonly DataContext _dataContext;
        public AddUpdateServiceTermCommandHandler(
            ILoggerService loggerService, 
            ISupplierRepository supplierRepository,  
            IIdentityServerRequest serverRequest,
            DataContext  dataContext)
            
        {
            _logger = loggerService;
            _serverRequest = serverRequest;
            _dataContext = dataContext;
            _supRepo = supplierRepository; 
        }
        public async Task<ServiceTermRegRespObj> Handle(AddUpdateServiceTermCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _serverRequest.UserDataAsync();
                if(user == null)
                {
                    return new ServiceTermRegRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = $"Unable to process user details",
                            }
                        }
                    };
                }

                var data = _dataContext.cor_serviceterms.Where(w => w.Header.Trim().ToLower() == request.Header.Trim().ToLower() && request.ServiceTermsId != w.ServiceTermsId).ToList();
                
                if (data.Count() > 1)
                {
                    return new ServiceTermRegRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = $"Header Already exist",
                            }
                        }
                    };
                }

                cor_serviceterms sup = new cor_serviceterms();
                sup.Deleted = true;
                sup.CreatedOn = request.ServiceTermsId > 0 ? (DateTime?)null : DateTime.Now;
                sup.CreatedBy = user.UserName;
                sup.UpdatedBy = user.UserName;
                sup.Header = request.Header;
                sup.Content = request.Content;
                sup.Deleted = false;
                if(request.ServiceTermsId > 0)
                {
                    sup.ServiceTermsId = request.ServiceTermsId;
                }
                await _supRepo.AddUpdateSeviceTermAsync(sup);
                var actionTaken = request.ServiceTermsId < 1 ? "created" : "updated";
                return new ServiceTermRegRespObj
                {
                    ServiceTermId = sup.ServiceTermsId,
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = $"Successfully  {actionTaken}",
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new ServiceTermRegRespObj
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Unable to process request",
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
