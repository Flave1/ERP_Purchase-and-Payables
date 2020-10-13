using AutoMapper;
using GODP.APIsContinuation.DomainObjects.Supplier; 
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier; 
using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using Microsoft.AspNetCore.Http; 
using System; 
using System.Threading;
using System.Threading.Tasks;
using GOSLibraries.GOS_Error_logger.Service;
using Puchase_and_payables.AuthHandler;
using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Requests;
using System.IO; 
using Puchase_and_payables.Data; 

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class UpdateSupplierDocumentCommandHandler : IRequestHandler<UpdateSupplierDocumentCommand, SupplierDocumentRegRespObj>
    {
        private readonly ILoggerService _logger;
        private readonly ISupplierRepository _supRepo; 
        private readonly IHttpContextAccessor _httpContextAccessor;  
        public UpdateSupplierDocumentCommandHandler(
            ILoggerService loggerService,  
            ISupplierRepository supplierRepository,
            IHttpContextAccessor httpContextAccessor )
        { 
            _logger = loggerService;
            _supRepo = supplierRepository;  
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<SupplierDocumentRegRespObj> Handle(UpdateSupplierDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = new SupplierDocumentRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                
                var doc = _httpContextAccessor.HttpContext.Request.Form.Files;
                if (doc == null)
                {
                    apiResponse.Status.Message.FriendlyMessage = "Authorization file required";
                    return apiResponse;
                }
                if(doc[0].FileName.Split('.').Length > 2)
                {
                    apiResponse.Status.Message.FriendlyMessage = "Invalid Character detected in file Name";
                    return apiResponse;
                }
                var byteArray = new byte[0];
                foreach (var fileBit in doc)
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

                cor_supplierdocument supDoc = new cor_supplierdocument();
                  
                supDoc.SupplierId = request.SupplierId;
                supDoc.SupplierDocumentId = request.SupplierDocumentId; 
                supDoc.ReferenceNumber = request.ReferenceNumber;
                supDoc.Description = request.Description;
                supDoc.Document = byteArray;
                supDoc.FileType = doc[0].ContentType;
                supDoc.Extension = doc[0].FileName.Split('.')[1];
                supDoc.DocumentId = request.DocumentId;
                supDoc.Active = true;
                supDoc.Deleted = false; 
                await _supRepo.UpdateSupplierDocumentAsync(supDoc); 
              
                return new SupplierDocumentRegRespObj
                {
                    SupplierDocumentId = supDoc.SupplierDocumentId,
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
                return new SupplierDocumentRegRespObj
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Unable to Process request",
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
