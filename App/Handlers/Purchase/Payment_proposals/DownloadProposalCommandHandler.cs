using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;    
using Puchase_and_payables.Contracts.Response; 
using Puchase_and_payables.Data; 
using System; 
using System.IO; 
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class DownloadProposalQuery : IRequest<DownloadFIleResp>
    {
        public int BidAndTenderId { get; set; }
        public class DownloadProposalQueryHandler : IRequestHandler<DownloadProposalQuery, DownloadFIleResp>
        { 
            private readonly ILoggerService _logger;  
            private readonly DataContext _dataContext; 
            public DownloadProposalQueryHandler( 
                ILoggerService loggerService, 
                DataContext dataContext)
            {
                _logger = loggerService;  
                _dataContext = dataContext;
            }
            public async Task<DownloadFIleResp> Handle(DownloadProposalQuery request, CancellationToken cancellationToken)
            {
                var response = new DownloadFIleResp { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    if (request.BidAndTenderId < 1)
                    { 
                        response.Status.Message.FriendlyMessage = "Error Ocurred! Bid Not Found";
                        return response;
                    }
                    var bid = await _dataContext.cor_bid_and_tender.FindAsync(request.BidAndTenderId);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + bid.ProposalTenderUploadName);

                    if (string.IsNullOrEmpty(bid.ProposalTenderUploadName) || path.Length < 1)
                    {
                        response.Status.Message.FriendlyMessage = "File Not Found";
                        return response;
                    } 
                    response.FileName = bid?.ProposalTenderUploadName;
                    response.FIle = File.ReadAllBytes(path);
                    response.Extension = bid?.Extention;
                    response.Status.IsSuccessful = true;
                    return response;
                }
                catch (Exception ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    response.Status.IsSuccessful = false; 
                    response.Status.Message.FriendlyMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                    response.Status.Message.TechnicalMessage = ex.ToString();
                    return response;
                    #endregion
                }
            }
          
        }
    } 
}
