using AutoMapper;
using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Supplier.setup;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier
{
      public class AddUpdateSuppliertypeCommandHandler : IRequestHandler<AddUpdateSuppliertypeCommand, SuppliertypeRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly IIdentityService _identityService;
        private readonly IIdentityServerRequest _serverRequest;
        public AddUpdateSuppliertypeCommandHandler(ILoggerService loggerService,
            ISupplierRepository supplierRepository,
            IIdentityService identityService,
            IIdentityServerRequest serverRequest)
        {
            _logger = loggerService;
            _supRepo = supplierRepository;
            _identityService = identityService;
            _serverRequest = serverRequest;
        }
        public async Task<SuppliertypeRegRespObj> Handle(AddUpdateSuppliertypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _serverRequest.UserDataAsync();
                cor_suppliertype sup = new cor_suppliertype();
                sup.Deleted = true;
                sup.CreatedOn = request.SupplierTypeId > 0 ? (DateTime?)null : DateTime.Now;
                sup.CreatedBy = user.UserName;
                sup.UpdatedBy = user.UserName;
                sup.SupplierTypeName = request.SupplierTypeName;
                sup.TaxApplicable = string.Join(',', request.TaxApplicable);
                sup.CreditGL = request.CreditGL;
                sup.DebitGL = request.DebitGL;
                sup.Deleted = false;
                sup.SupplierTypeId = request.SupplierTypeId > 0 ? request.SupplierTypeId : 0;
                await _supRepo.AddUpdateSupplierTypeAsync(sup);
                var actionTaken = request.SupplierTypeId < 1 ? "created" : "updated";
                return new SuppliertypeRegRespObj
                {
                    SuppliertypeId = sup.SupplierTypeId,
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
                return new SuppliertypeRegRespObj
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
