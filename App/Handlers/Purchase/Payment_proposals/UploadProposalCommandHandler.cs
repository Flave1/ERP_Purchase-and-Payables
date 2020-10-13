using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class UploadProposalCommand : IRequest<BidAndTenderRegRespObj>
    { 
        public int SupplierId { get; set; }
        public string LPONumber { get; set; }
        public IFormFile Proposal { get; set; }
        public class UploadProposalCommandHandler : IRequestHandler<UploadProposalCommand, BidAndTenderRegRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly ILoggerService _logger;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IHttpContextAccessor _accessor;
            private readonly DataContext _dataContext;
            private readonly IWebHostEnvironment _env;
            public UploadProposalCommandHandler(
                IPurchaseService purchaseService,
                ILoggerService loggerService,
                IWebHostEnvironment webHostEnvironment,
                DataContext dataContext,
                IHttpContextAccessor httpContextAccessor,
                IIdentityServerRequest serverRequest)
            {
                _logger = loggerService;
                _env = webHostEnvironment;
                _accessor = httpContextAccessor;
                _repo = purchaseService;
                _serverRequest = serverRequest;
                _dataContext = dataContext;
            }
            public async Task<BidAndTenderRegRespObj> Handle(UploadProposalCommand request, CancellationToken cancellationToken)
            {
                var response = new BidAndTenderRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var user = await _serverRequest.UserDataAsync();
                    if (user == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Process This User";
                        return response;
                    }

                     
                    if (string.IsNullOrEmpty(request.LPONumber) ||  request.SupplierId < 1)
                    {
                        response.Status.Message.FriendlyMessage = "Supplier Number and Identification is required to process this request";
                        return response;
                    }

                    var bidAndTenderObj = _dataContext.cor_bid_and_tender.FirstOrDefault(w => w.SupplierId == request.SupplierId && w.LPOnumber.Trim().ToLower() == request.LPONumber.Trim().ToLower());
                    if(bidAndTenderObj == null)
                    {
                        bidAndTenderObj = new cor_bid_and_tender();
                        bidAndTenderObj.ApprovalStatusId = (int)ApprovalStatus.Authorised;
                        bidAndTenderObj.DecisionResult = (int)DecisionResult.Non_Applicable;
                    }
                    var file = _accessor.HttpContext.Request.Form.Files;
                    if (file.Count() > 0)
                    {

                        if (file[0].FileName.Split('.').Length > 2)
                        {
                            response.Status.Message.FriendlyMessage = "Invalid Character detected in file Name";
                            return response;
                        }

                        var folderName = Path.Combine("Resources", "Images");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var fileName = $"{request.LPONumber}-{request.SupplierId}-{DateTime.Now.ToString().Split(" ")[1].Replace(':','-')}"; 
                        var type = file[0].ContentType; 



                        var fullPath = _env.WebRootPath + "/Resources/" + fileName;
                        var dbPath = _env.WebRootPath + "/Resources/" + fileName;

                        using (FileStream filestrem = System.IO.File.Create(fullPath))
                        {
                            await file[0].CopyToAsync(filestrem);
                            await filestrem.FlushAsync();
                        }
                         
                        bidAndTenderObj.ProposalTenderUploadFullPath = fullPath;
                        bidAndTenderObj.ProposalTenderUploadName = "/Resources/" + fileName;
                        bidAndTenderObj.ProposalTenderUploadPath = dbPath;
                        bidAndTenderObj.ProposalTenderUploadType = type;
                        bidAndTenderObj.Extention = file[0].FileName.Split('.')[1];
                        bidAndTenderObj.SupplierId = request.SupplierId;
                        bidAndTenderObj.LPOnumber = request.LPONumber;
                    }
                    bidAndTenderObj.Paymentterms = _dataContext.cor_paymentterms.Where(w => w.BidAndTenderId == bidAndTenderObj.BidAndTenderId).ToList();
                    if(bidAndTenderObj.Paymentterms.Count() ==0)
                    {
                        bidAndTenderObj.Paymentterms = new List<cor_paymentterms>();
                    }
                    await _repo.AddUpdateBidAndTender(bidAndTenderObj);
                    response.Status.IsSuccessful = true; 
                    response.Status.Message.FriendlyMessage = "Successful";
                    return response;
                }
                catch (Exception ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    response.Status.IsSuccessful = true; 
                    response.Status.Message.FriendlyMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                    response.Status.Message.TechnicalMessage = ex.ToString();
                    return response; 
                    #endregion
                }
            }
          
        }
    } 
}
