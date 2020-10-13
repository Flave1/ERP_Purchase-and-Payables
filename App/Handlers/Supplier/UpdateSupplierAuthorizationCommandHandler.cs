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
using System.IO;
using System.Net.Http;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class UpdateSupplierAuthorizationCommandHandler : IRequestHandler<UpdateSupplierAuthorizationCommand, SupplierAuthorizationRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;  
        private readonly IIdentityServerRequest _serverRequest;
        private readonly IHttpContextAccessor _acceossor;
        public UpdateSupplierAuthorizationCommandHandler( 
            ILoggerService loggerService, 
            IMapper mapper, 
            ISupplierRepository supplierRepository,   
            IIdentityServerRequest serverRequest,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _logger = loggerService;
            _supRepo = supplierRepository; 
            _serverRequest = serverRequest;
            _acceossor = httpContextAccessor;
        }
        public async Task<SupplierAuthorizationRegRespObj> Handle(UpdateSupplierAuthorizationCommand request, CancellationToken cancellationToken)
        {
			try
			{
                var apiResponse = new SupplierAuthorizationRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                var byteArray = new byte[0];
                if(request.SupplierAuthorizationId > 0)
                {
                    var item = await _supRepo.GetSupplierAuthorizationAsync(request.SupplierAuthorizationId);
                    if(item != null)
                    {
                        byteArray = item.Signature;
                    }
                }
                var signature = _acceossor.HttpContext.Request.Form.Files;
                if (signature != null)
                {
                    foreach (var fileBit in signature)
                    {
                        if (fileBit.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                await fileBit.CopyToAsync(ms);
                                byteArray = ms.ToArray();
                            }
                        }
                    }
                }

                cor_supplierauthorization supAuth = new cor_supplierauthorization(); 
                supAuth.CreatedOn = request.SupplierAuthorizationId > 0? (DateTime?)null : DateTime.Now;
                supAuth.Signature = byteArray;  
                supAuth.Address = request.Address;
                supAuth.Email = request.Email;
                supAuth.Name = request.Name;
                supAuth.PhoneNo = request.PhoneNo; 
                supAuth.SupplierAuthorizationId = request.SupplierAuthorizationId;
                supAuth.SupplierId = request.SupplierId; 

                await _supRepo.UpdateSupplierAuthorizationAsync(supAuth);
                apiResponse.SupplierAuthorizationId = supAuth.SupplierAuthorizationId;
                apiResponse.Status.IsSuccessful = true;
                apiResponse.Status.Message.FriendlyMessage = "Successfully created";
                return apiResponse;
            }
			catch (Exception ex)
			{
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new SupplierAuthorizationRegRespObj
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
