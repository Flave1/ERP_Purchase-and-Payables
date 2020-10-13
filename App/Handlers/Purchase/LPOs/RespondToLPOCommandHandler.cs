using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase.LPOs
{
    public class RespondToLPOCommand : IRequest<LPORegRespObj>
    {
        public int LPOId { get; set; } 
        public bool IsRejected { get; set; }
        public class RespondToLPOCommandHandler : IRequestHandler<RespondToLPOCommand, LPORegRespObj>
        {
            private readonly DataContext _dataContext;
            public readonly IPurchaseService _purchaseService;
            public readonly IIdentityServerRequest _serverRequest;
            public readonly ILoggerService _logger;
            public RespondToLPOCommandHandler(
                DataContext dataContext, 
                IPurchaseService purchaseService,
                IIdentityServerRequest request,
                ILoggerService loggerService)
            {
                _purchaseService = purchaseService;
                _serverRequest = request;
                _logger = loggerService;
                _dataContext = dataContext;
            }
            public async Task<LPORegRespObj> Handle(RespondToLPOCommand request, CancellationToken cancellationToken)
            {
                var response = new LPORegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var rejectedlpo = _dataContext.purch_plpo.Find(request.LPOId);
                    if(rejectedlpo == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable to identify this LPO";
                        return response;
                    }
                    var rejectedLpoBid = await _purchaseService.GetBidAndTender(rejectedlpo.BidAndTenderId);
                    if (rejectedLpoBid == null)
                    {
                        response.Status.Message.FriendlyMessage = "Error Occurred";
                        return response;
                    }
                    
                    if (request.IsRejected)
                    {
                        using (var _trans = await _dataContext.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                await _purchaseService.SendEmailToSuppliersWhenBidIsRejectedAsync(rejectedlpo.WinnerSupplierId, rejectedlpo.Description);

                                rejectedlpo.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
                                rejectedlpo.WinnerSupplierId = 0;
                                rejectedlpo.Name = string.Empty;
                                rejectedlpo.BidAndTenderId = 0;
                                await _purchaseService.AddUpdateLPOAsync(rejectedlpo);
                                

                                if (rejectedLpoBid.Paymentterms.Count() > 0)
                                {
                                    foreach (var term in rejectedLpoBid.Paymentterms)
                                    {
                                        var item = await _dataContext.cor_paymentterms.FindAsync(term.PaymentTermId);
                                        if (item != null)
                                        {
                                            item.LPOId = 0;
                                            item.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
                                        }
                                        await _purchaseService.AddUpdatePaymentTermsAsync(item);
                                    }
                                }
                                rejectedLpoBid.IsRejected = true;
                                rejectedLpoBid.PLPOId = 0;
                                rejectedLpoBid.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
                                await _purchaseService.AddUpdateBidAndTender(rejectedLpoBid);
                                var lostBids = await _dataContext.cor_bid_and_tender.Where(q => q.PLPOId == request.LPOId && q.BidAndTenderId != rejectedLpoBid.BidAndTenderId).ToListAsync();
                                if (lostBids.Count() > 0)
                                {
                                    foreach (var otherbid in lostBids)
                                    {
                                        var item = await _purchaseService.GetBidAndTender(otherbid.BidAndTenderId);
                                        if (item != null)
                                        { 
                                            item.ApprovalStatusId = (int)ApprovalStatus.Pending;
                                            item.DecisionResult = (int)DecisionResult.Non_Applicable;
                                            var terms = await _dataContext.cor_paymentterms.Where(q => q.BidAndTenderId == item.BidAndTenderId).ToListAsync();
                                            item.Paymentterms = terms;
                                        }
                                        await _purchaseService.AddUpdateBidAndTender(item);
                                    }
                                }
                                await _trans.CommitAsync();
                                response.Status.IsSuccessful = true;
                                response.Status.Message.FriendlyMessage = "Successfully rejected LPO"; 
                                return response;
                            }
                            catch (Exception ex)
                            {
                                await _trans.RollbackAsync();
                            }
                            finally { await _trans.DisposeAsync(); }
                        }
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    response.Status.Message.FriendlyMessage = ex.Message;
                    return response;
                }
            }
        }
    }
    
}
