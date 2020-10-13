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
    public class AddUpdateTaxsetupCommandHandler : IRequestHandler<AddUpdateTaxsetupCommand, TaxsetupRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly DataContext _data;

        private readonly IIdentityServerRequest _serverRequest;
        public AddUpdateTaxsetupCommandHandler(
            ILoggerService loggerService, 
            IIdentityServerRequest serverRequest,
            ISupplierRepository supplierRepository,
            DataContext data)
        {
            _logger = loggerService;
            _supRepo = supplierRepository;
            _serverRequest = serverRequest;
            _data = data;
        }
        public async Task<TaxsetupRegRespObj> Handle(AddUpdateTaxsetupCommand request, CancellationToken cancellationToken)
        {
            var response = new TaxsetupRegRespObj { Status =new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage()} };  

            try
            {
                if(request.TaxSetupId < 1)
                {
                    var item = _data.cor_taxsetup.Any(q => q.TaxName.Trim().ToLower() == request.TaxName.Trim().ToLower());
                    if (item)
                    {
                        response.Status.IsSuccessful = false;
                        response.Status.Message.FriendlyMessage = $"Name Already exist";
                        return response;
                    }
                }
                string[] Types = { "+", "-" };
                if (!Types.Contains(request.Type))
                {
                    response.Status.Message.FriendlyMessage = "Invalid Tax type";
                    return response;
                }
                 

                var user = await _serverRequest.UserDataAsync();
                cor_taxsetup sup = new cor_taxsetup();
                sup.Deleted = true;
                sup.CreatedOn = request.TaxSetupId > 0 ? (DateTime?)null : DateTime.Now;
                sup.CreatedBy = user.UserName;
                sup.UpdatedBy = user.UserName;
                sup.Percentage = request.Percentage;
                sup.SubGL = request.SubGL;
                sup.Type = request.Type;
                sup.Deleted = false;
                sup.TaxName = request.TaxName;
                sup.TaxSetupId = request.TaxSetupId > 0 ? request.TaxSetupId : 0;
                await _supRepo.AddUpdateTaxSetupAsync(sup);


                var actionTaken = request.TaxSetupId < 1 ? "created" : "updated";
                response.TaxSetupId = sup.TaxSetupId;
                response.Status.IsSuccessful = true;
                response.Status.Message.FriendlyMessage = $"Successfully ";
                return response;

            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");

                response.Status.IsSuccessful = false;
                response.Status.Message.FriendlyMessage = "Error occured!! Unable to delete item";
                response.Status.Message.TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                return response;
                #endregion
            }
        }
    }
}
