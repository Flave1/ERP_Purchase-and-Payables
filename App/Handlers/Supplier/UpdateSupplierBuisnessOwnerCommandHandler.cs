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
using System.IO;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class UpdateSupplierBuisnessOwnerCommandHandler : IRequestHandler<UpdateSupplierBuisnessOwnerCommand, SupplierBuisnessOwnerRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper; 
        private readonly IHttpContextAccessor _accessor;
        private readonly IIdentityService _identityService;

        private readonly IIdentityServerRequest _serverRequest;
        public UpdateSupplierBuisnessOwnerCommandHandler(
            ILoggerService loggerService, 
            IMapper mapper, 
            ISupplierRepository supplierRepository,
           IHttpContextAccessor httpContextAccessor, 
           IIdentityService identityService,
            IIdentityServerRequest serverRequest)
        {
            _mapper = mapper;
            _logger = loggerService;
            _supRepo = supplierRepository;
            _accessor = httpContextAccessor;
            _identityService = identityService;
            _serverRequest = serverRequest;
        }
        public async Task<SupplierBuisnessOwnerRegRespObj> Handle(UpdateSupplierBuisnessOwnerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var files = _accessor.HttpContext.Request.Form.Files;

                var byteList = new byte[0]; 
                if (request.SupplierBusinessOwnerId > 0)
                {
                    var item = await _supRepo.GetSupplierBusinessOwnerAsync(request.SupplierBusinessOwnerId);
                    if (item != null)
                    {
                        byteList = item.Signature;
                    }
                }

                foreach (var fileBit in files)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteList = ms.ToArray();
                        }
                    }

                }


                var supBusOwner = new cor_supplierbusinessowner();  
                supBusOwner.Address = request.Address;
                supBusOwner.DateOfBirth = request.DateOfBirth;
                supBusOwner.Email = request.Email;
                supBusOwner.Name = request.Name;
                supBusOwner.PhoneNo = request.PhoneNo;
                supBusOwner.Signature = byteList;
                supBusOwner.SupplierBusinessOwnerId = request.SupplierBusinessOwnerId;
                supBusOwner.SupplierId = request.SupplierId; 
                await _supRepo.UpdateSupplierBusinessOwnerAsync(supBusOwner);

                return new SupplierBuisnessOwnerRegRespObj
                {
                    SupplierBuisnessOwnerId = supBusOwner.SupplierId,
                    Status = new APIResponseStatus{ IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Successfully created",
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : SupplierBuisnessOwnerCommandHandler{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new SupplierBuisnessOwnerRegRespObj
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Unable to process item",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : SupplierBuisnessOwnerCommandHandler{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }
    }
}
