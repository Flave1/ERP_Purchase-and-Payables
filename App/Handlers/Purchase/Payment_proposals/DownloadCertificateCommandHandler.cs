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
    public class DownloadCertificateQuery : IRequest<DownloadFIleResp>
    {
        public DownloadCertificateQuery() { }
        public int paymenttermid { get; set; }
        public DownloadCertificateQuery(int termid)
        {
            paymenttermid = termid;
        }
       
        public class DownloadCertificateQueryHandler : IRequestHandler<DownloadCertificateQuery, DownloadFIleResp>
        { 
            private readonly ILoggerService _logger;  
            private readonly DataContext _dataContext; 
            public DownloadCertificateQueryHandler( 
                ILoggerService loggerService, 
                DataContext dataContext)
            {
                _logger = loggerService;  
                _dataContext = dataContext;
            }
            public async Task<DownloadFIleResp> Handle(DownloadCertificateQuery request, CancellationToken cancellationToken)
            {
                var response = new DownloadFIleResp { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    if (request.paymenttermid < 1)
                    { 
                        response.Status.Message.FriendlyMessage = "Error Ocurred! Phase Not Found";
                        return response;
                    }
                    var phase = await _dataContext.cor_paymentterms.FindAsync(request.paymenttermid);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + phase.CcompletionCertificateName);

                    if (string.IsNullOrEmpty(phase.CcompletionCertificateName) || path.Length < 1)
                    {
                        response.Status.Message.FriendlyMessage = "File Not Found";
                        return response;
                    } 
                    response.FileName = phase?.CcompletionCertificateName;
                    response.FIle = File.ReadAllBytes(path);
                    response.Extension = phase?.CcompletionCertificateType;
                    response.Status.IsSuccessful = true;
                    return response;
                }
                catch (Exception ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = ex?.Message;
                    response.Status.Message.TechnicalMessage = ex.ToString();
                    return response;
                    #endregion
                }
            }
          
        }
    } 
}
