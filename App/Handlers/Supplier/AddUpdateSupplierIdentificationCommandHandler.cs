using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier; 
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.AspNetCore.Http; 
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Requests;
using System; 
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier
{ 

    public class AddUpdateSupplierIdentificationCommandHandler : IRequestHandler<AddUpdateSupplierIdentificationCommand, IdentificationRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo; 
        private readonly IHttpContextAccessor _httpContextAccessor;  
        private readonly IIdentityServerRequest _serverRequest;
        public AddUpdateSupplierIdentificationCommandHandler(
            ILoggerService loggerService,  
            ISupplierRepository supplierRepository,
           IHttpContextAccessor httpContextAccessor, 
           IIdentityServerRequest serverRequest)
        { 
            _logger = loggerService; 
            _supRepo = supplierRepository;
            _serverRequest = serverRequest;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IdentificationRegRespObj> Handle(AddUpdateSupplierIdentificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = new IdentificationRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                if(request.IsCorporate && (string.IsNullOrEmpty(request.RegistrationNumber) || request.IncorporationDate == null || request.BusinessType < 1))
                {
                    apiResponse.Status.Message.FriendlyMessage = "Reg. Number, Date of Incorporation and Business Type is reqiuired for a Corporate Identity";
                    return apiResponse;
                }

                if(request.IsCorporate &&  request.BusinessType == (int)BusinessType.Others && string.IsNullOrEmpty(request.OtherBusinessType))
                {
                    apiResponse.Status.Message.FriendlyMessage = "Other business type is required";
                    return apiResponse;
                }

                if (!request.IsCorporate  && (request.Identification < 1 || string.IsNullOrEmpty(request.Identification_Number) || request.Expiry_Date == null || request.Nationality < 1))
                {
                    apiResponse.Status.Message.FriendlyMessage = "Identification Number, Expiry Date, and Nationality is required for a non-corporate Identity";
                    return apiResponse;
                }

                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
                UserDataResponseObj user = null;

                user = await _serverRequest.UserDataAsync();

                cor_identification item = new cor_identification();
                item.Deleted = false;
                item.CreatedOn = request.SupplierId > 0 ? (DateTime?)null : DateTime.Now;
                item.CreatedBy = user != null ? user.UserName : "";
                item.UpdatedBy = user != null ? user.UserName : "";
                item.Expiry_Date = request.Expiry_Date ?? (DateTime?)null;
                item.HaveWorkPermit = request.HaveWorkPermit; 
                item.Identification = request.Identification;
                item.SupplierId = request.SupplierId;
                item.Identification_Number = request.Identification_Number;
                item.IdentificationId = request.IdentificationId;
                item.IncorporationDate = request.IncorporationDate ?? (DateTime?)null;
                item.IsCorporate = request.IsCorporate;
                item.Nationality = request.Nationality;
                item.OtherBusinessType = request.OtherBusinessType;
                item.RegistrationNumber = request.RegistrationNumber;
                item.BusinessType = request.BusinessType;

                await _supRepo.AddUpdateSupplierIdentificationAsync(item);

                apiResponse.Status.IsSuccessful = true;
                apiResponse.Status.Message.FriendlyMessage = "Successful";
                return apiResponse; 
            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new IdentificationRegRespObj
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
