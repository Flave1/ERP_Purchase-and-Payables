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
using GOSLibraries.GOS_Error_logger.Service;
using Puchase_and_payables.AuthHandler;
using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Requests;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class UpdateSupplierTopSupplierCommandHandler : IRequestHandler<UpdateSupplierTopSupplierCommand, SupplierTopSupplierRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper; 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;

        private readonly IIdentityServerRequest _serverRequest;
        public UpdateSupplierTopSupplierCommandHandler(ILoggerService loggerService, IMapper mapper, ISupplierRepository supplierRepository,
           IHttpContextAccessor httpContextAccessor, IIdentityServerRequest serverRequest, IIdentityService identityService)
        {
            _mapper = mapper;
            _logger = loggerService;
            _supRepo = supplierRepository;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _serverRequest = serverRequest;
        }
        public async Task<SupplierTopSupplierRegRespObj> Handle(UpdateSupplierTopSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            { 
                var user = await _serverRequest.UserDataAsync();

                cor_topsupplier supTopSupplier = new cor_topsupplier();
                supTopSupplier.Deleted = false;
                supTopSupplier.CreatedOn = request.SupplierId > 0 ? (DateTime?)null : DateTime.Now;
                supTopSupplier.CreatedBy = user != null ? user.UserName : "";
                supTopSupplier.UpdatedBy = user != null ? user.UserName : "";
                supTopSupplier.SupplierId = request.SupplierId;
                supTopSupplier.TopSupplierId = request.TopSupplierId;
                supTopSupplier.PhoneNo = request.PhoneNo;
                supTopSupplier.Name = request.Name;
                supTopSupplier.Address = request.Address;
                supTopSupplier.ContactPerson = request.ContactPerson;
                supTopSupplier.Email = request.Email;
                supTopSupplier.NoOfStaff = request.NoOfStaff; 
                supTopSupplier.CountryId = request.CountryId;
                await _supRepo.UpdateSupplierTopSupplierAsync(supTopSupplier);
                return new SupplierTopSupplierRegRespObj
                {
                    TopSupplierId = supTopSupplier.TopSupplierId,
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
                return new SupplierTopSupplierRegRespObj
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
