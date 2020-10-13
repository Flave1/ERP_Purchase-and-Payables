using AutoMapper;
using GODP.APIsContinuation.DomainObjects.Supplier; 
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier; 
using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Requests;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class UpdateSupplierTopClientCommandHandler : IRequestHandler<UpdateSupplierTopClientCommand, SupplierTopClientRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper; 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;
        private readonly IIdentityServerRequest _serverRequest;
        public UpdateSupplierTopClientCommandHandler(
            ILoggerService loggerService, 
            IMapper mapper, 
            ISupplierRepository supplierRepository,
           IHttpContextAccessor httpContextAccessor, 
           IIdentityService identityService,
           IIdentityServerRequest serverRequest)
        {
            _mapper = mapper;
            _serverRequest = serverRequest;
            _logger = loggerService;
            _identityService = identityService;
            _supRepo = supplierRepository; 
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<SupplierTopClientRegRespObj> Handle(UpdateSupplierTopClientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
                var user = await _serverRequest.UserDataAsync();
                cor_topclient supTopClient = new cor_topclient();
                supTopClient.Deleted = false;
                supTopClient.CreatedOn = request.SupplierId > 0 ? (DateTime?)null : DateTime.Now;
                supTopClient.CreatedBy = user != null ? user.UserName : "";
                supTopClient.UpdatedBy = user != null ? user.UserName : "";
                supTopClient.Address = request.Address;
                supTopClient.ContactPerson = request.ContactPerson;
                supTopClient.CreatedBy = request.CreatedBy;
                supTopClient.CreatedOn = DateTime.Today;
                supTopClient.Email = request.Email;
                supTopClient.Name = request.Name;
                supTopClient.NoOfStaff = request.NoOfStaff;
                supTopClient.PhoneNo = request.PhoneNo;
                supTopClient.SupplierId = request.SupplierId;
                supTopClient.TopClientId = request.TopClientId; 
                supTopClient.CountryId = request.CountryId;
                
                await _supRepo.UpdateSupplierTopClientAsync(supTopClient);
                return new SupplierTopClientRegRespObj
                {
                    TopClientId = supTopClient.TopClientId,
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
                return new SupplierTopClientRegRespObj
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
