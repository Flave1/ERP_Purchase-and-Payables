using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.Payment; 
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{

    public class SaveUpdatePaymentTermsCommand : IRequest<PaymentTermsRegRespObj>
    { 
        public int BidAndTenderId { get; set; } 
        public decimal TotalAmount { get; set; }
        public List<UpdateTerms> Terms { get; set; }
        public class SaveUpdatePaymentTermsCommandHandler : IRequestHandler<SaveUpdatePaymentTermsCommand, PaymentTermsRegRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly ILoggerService _logger;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IFinanceServerRequest _financeServer;
            public SaveUpdatePaymentTermsCommandHandler(
                IPurchaseService purchaseService,
                ILoggerService loggerService, 
                IIdentityServerRequest serverRequest,
                IFinanceServerRequest financeServer)
            {
                _logger = loggerService;
                _financeServer = financeServer;
                _repo = purchaseService;
                _serverRequest = serverRequest; 
            }
            public async Task<PaymentTermsRegRespObj> Handle(SaveUpdatePaymentTermsCommand request, CancellationToken cancellationToken)
            {
                var response = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var user = await _serverRequest.UserDataAsync();
                    if (user == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Process This User";
                        return response; 
                    } 
                    var thisProposalBid = await _repo.GetBidAndTender(request.BidAndTenderId);
                    if(thisProposalBid == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Identify Bid associated to this payment proposal (s)";
                        return response;
                    }
                    if(request.Terms.Count() > 0)
                    {
                        foreach (var item in request.Terms)
                        {
                            
                            if(item.PaymentTermId > 0)
                            {
                                var existingTerm = await _repo.GetSinglePaymenttermAsync(item.PaymentTermId);
                                existingTerm.Phase = item.Phase;
                                existingTerm.BidAndTenderId = request.BidAndTenderId;
                                existingTerm.ProjectStatusDescription = item.ProjectStatusDescription;
                                existingTerm.ProposedBy = user.StaffId > 0 ? (int)Proposer.STAFF : (int)Proposer.SUPPLIER;
                                existingTerm.Comment = item.Comment;
                                existingTerm.GrossAmount = item.GrossAmount;
                                existingTerm.Completion = item.Completion;
                                existingTerm.CompanyId = thisProposalBid.CompanyId;
                                existingTerm.Payment = item.Payment;
                                existingTerm.LPOId = thisProposalBid.PLPOId;
                                existingTerm.Amount = item.Amount;
                                thisProposalBid.Paymentterms.Add(existingTerm);
                            }
                            else
                            {
                                var term = new cor_paymentterms
                                {
                                    Phase = item.Phase,
                                    BidAndTenderId = request.BidAndTenderId,
                                    ProjectStatusDescription = item.ProjectStatusDescription,
                                    ProposedBy =  user.StaffId > 0 ? (int)Proposer.STAFF : (int)Proposer.SUPPLIER, 
                                    Comment = item.Comment,
                                    GrossAmount = item.GrossAmount,
                                    Completion = item.Completion,
                                    CompanyId = thisProposalBid.CompanyId,
                                    Payment = item.Payment, 
                                    Amount = item.Amount,
                                    LPOId = thisProposalBid.PLPOId,
                            }; 
                                thisProposalBid.Paymentterms.Add(term);
                            }
                            thisProposalBid.Total = request.TotalAmount; 
                            await _repo.AddUpdateBidAndTender(thisProposalBid); 
                        } 
                    }
                    response.Status.IsSuccessful = true;
                    response.Status.Message.FriendlyMessage = "Successful";
                    return response;
                }
                catch (Exception ex)
                {
                    #region Log error to file 
                    var errorCode = ErrorID.Generate(4);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    return new PaymentTermsRegRespObj
                    {

                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Error occured!! Unable to process item",
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
    
}