using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier; 
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.Data.SqlClient;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Data; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class DeleteSupplierDocumentCommandHandler : IRequestHandler<DeleteSupplierDocumentCommand, DeleteRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly ILoggerService _logger;
        private readonly DataContext _dataContext;
        public DeleteSupplierDocumentCommandHandler(ISupplierRepository supplierRepository, DataContext dataContext, ILoggerService loggerService)
        {
            _supRepo = supplierRepository;
            _dataContext = dataContext;
            _logger = loggerService;
        }
        public async Task<DeleteRespObj> Handle(DeleteSupplierDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.req.Count() > 0)
                {
                    foreach (var itemId in request.req)
                    {
                        var itemToDelete = await _supRepo.GetSupplierDocumentAsync(itemId.TargetId);
                        await _supRepo.DeleteSupplierDocumentAsync(itemToDelete);
                    }
                }
                return new DeleteRespObj
                {
                    Deleted = true,
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Item(s) deleted succcessfully",
                        }
                    }
                };

            }
            catch (SqlException ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new DeleteRespObj
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Unable to process required",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }
    }
}
