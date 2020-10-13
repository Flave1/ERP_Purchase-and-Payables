using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service; 
using MediatR;
using Microsoft.Data.SqlClient;
using Puchase_and_payables.Contracts.Response; 
using Puchase_and_payables.Repository.Purchase; 
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class DeletePaymentProposalCommand : IRequest<DeleteRespObj>
    {
        public List<int> TargetIds { get; set; }
        public class DeletePaymentProposalCommandHandler : IRequestHandler<DeletePaymentProposalCommand, DeleteRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly ILoggerService _logger; 
        public DeletePaymentProposalCommandHandler(IPurchaseService  purchaseService, ILoggerService loggerService)
        {
                _repo = purchaseService;
                _logger = loggerService;
        }
        public async Task<DeleteRespObj> Handle(DeletePaymentProposalCommand request, CancellationToken cancellationToken)
        {
                var response = new DeleteRespObj { Deleted = false, Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    if (request.TargetIds.Count() > 0)
                    {
                        foreach (var itemId in request.TargetIds)
                        {
                            var item = await _repo.GetSinglePaymenttermAsync(itemId);
                            if(item != null)
                            {
                                if(item.PaymentStatus == (int)PaymentStatus.Paid)
                                {
                                    response.Status.Message.FriendlyMessage = "Unable to delete paid proposal";
                                    return response;
                                }
                            }
                           await  _repo.DeletePaymentProposalAsync(itemId);
                        }
                    }
                    else
                    {
                        response.Status.Message.FriendlyMessage = "No Proposal Selected";
                        return response;
                    }
                    response.Deleted = true;
                    response.Status.IsSuccessful = true;
                    response.Status.Message.FriendlyMessage = "Item(s) deleted succcessfully";
                    return response; 
                }
                catch (SqlException ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    response.Status.Message.FriendlyMessage = "Error occured!! Unable to process item";
                    response.Status.Message.MessageId = errorCode;
                    response.Status.Message.TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                    return response;
                    #endregion
                }
            }
    }
    }
    
}
