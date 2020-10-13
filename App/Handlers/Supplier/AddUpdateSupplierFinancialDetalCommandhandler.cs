using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.AspNetCore.Http;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Supplier;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier
{
    public class AddUpdateSupplierFinancialDetalCommandHandler : IRequestHandler<AddUpdateSupplierFinancialDetalCommand, SupplierFinancialDetalRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo; 
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IIdentityServerRequest _serverRequest;

        public AddUpdateSupplierFinancialDetalCommandHandler(ILoggerService loggerService, 
            IMapper mapper, ISupplierRepository supplierRepository, IIdentityServerRequest serverRequest,
           IHttpContextAccessor httpContextAccessor, IIdentityService identityService)
        { 
            _logger = loggerService; 
            _supRepo = supplierRepository;
            _serverRequest = serverRequest;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<SupplierFinancialDetalRegRespObj> Handle(AddUpdateSupplierFinancialDetalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
                UserDataResponseObj user = null;

                user = await _serverRequest.UserDataAsync();

                cor_financialdetail item = new cor_financialdetail();
                item.Deleted = false;
                item.CreatedOn = request.SupplierId > 0 ? (DateTime?)null : DateTime.Now;
                item.CreatedBy = user != null ? user.UserName : "";
                item.UpdatedBy = user != null ? user.UserName : "";
                item.SupplierId = request.SupplierId;
                item.Value = request.Value;
                item.Year= request.Year;
                item.FinancialdetailId = request.FinancialdetailId;
                item.BusinessSize = request.BusinessSize;
                item.Active = true;

                await _supRepo.AddUpdateBankFinancialDetailsAsync(item);
                return new SupplierFinancialDetalRegRespObj
                {
                    FinancialdetailId = item.FinancialdetailId,
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Successful",
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new SupplierFinancialDetalRegRespObj
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
