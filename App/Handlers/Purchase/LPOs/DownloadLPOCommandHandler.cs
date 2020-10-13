using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;    
using Puchase_and_payables.Contracts.Response; 
using Puchase_and_payables.Data; 
using System; 
using System.IO; 
using System.Threading;
using System.Threading.Tasks;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Microsoft.AspNetCore.Hosting;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class DownloadLPOQuery : IRequest<DownloadFIleResp>
    {
        public int Lpoid { get; set; }
        public class DownloadLPOQueryHandler : IRequestHandler<DownloadLPOQuery, DownloadFIleResp>
        { 
            private readonly ILoggerService _logger;  
            private readonly DataContext _dataContext;
            private readonly IWebHostEnvironment _env;
            public DownloadLPOQueryHandler( 
                ILoggerService loggerService,
                IWebHostEnvironment webHostEnvironment,
                DataContext dataContext)
            {
                _logger = loggerService;
                _env = webHostEnvironment;
                _dataContext = dataContext;
            }
            public async Task<DownloadFIleResp> Handle(DownloadLPOQuery request, CancellationToken cancellationToken)
            {
                var response = new DownloadFIleResp { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    //if (request.Lpoid < 1)
                    //{ 
                    //    response.Status.Message.FriendlyMessage = "Error Ocurred! Lpo Not Found";
                    //    return response;
                    //}
                    var lpo = await _dataContext.purch_plpo.FindAsync(request.Lpoid);

                    var pathToFile = _env.WebRootPath + Path.DirectorySeparatorChar.ToString()
                          + "ReportTemplates" +  Path.DirectorySeparatorChar.ToString() + "login.html";
                     
                    using (Stream streamed = File.OpenRead(pathToFile))
                    {
                        using (WordDocument document = new WordDocument(streamed, FormatType.Html, XHTMLValidationType.None))
                        {
                            //Saves the Word document
                           document.Save(streamed, FormatType.Docx);
                        }                     
                    }
                 
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
