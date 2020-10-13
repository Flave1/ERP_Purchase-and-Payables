using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR; 
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Data;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System; 
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class GetRequestedPaymentsAwaitingApprovalQuery : IRequest<ProposalPaymentRespObj>
    {
        public class GetRequestedPaymentsAwaitingApprovalQueryHandler : IRequestHandler<GetRequestedPaymentsAwaitingApprovalQuery, ProposalPaymentRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly DataContext _dataContext;

            public GetRequestedPaymentsAwaitingApprovalQueryHandler(
                IPurchaseService Repository,
                DataContext dataContext,
                IIdentityServerRequest identityServerRequest)
            {
                _repo = Repository;
                _dataContext = dataContext;
                _serverRequest = identityServerRequest;
            }

            public async Task<ProposalPaymentRespObj> Handle(GetRequestedPaymentsAwaitingApprovalQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                    if (!result.IsSuccessStatusCode)
                    {
                        var data1 = await result.Content.ReadAsStringAsync();
                        var res1 = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data1);
                        return new ProposalPaymentRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                IsSuccessful = false,
                                Message = new APIResponseMessage { FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}" }
                            }
                        };
                    }

                    var data = await result.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data);

                    if (res == null)
                    {
                        return new ProposalPaymentRespObj
                        {
                            Status = res.Status
                        };
                    }

                    if (res.workflowTasks.Count() < 1)
                    {
                        return new ProposalPaymentRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                IsSuccessful = true,
                                Message = new APIResponseMessage
                                {
                                    FriendlyMessage = "No Pending Approval"
                                }
                            }
                        };
                    }
                    CompanyStructureRespObj _Department = new CompanyStructureRespObj();
                    _Department = await _serverRequest.GetAllCompanyStructureAsync();
                    var paymentTerms = await _repo.GetPaymenttermsAsync();
                    var bids = await _repo.GetAllBidAndTender();
                    var staffRequestedPaymentsawaiting = await _repo.GetRequestedPaymentsAwaitingApprovalAsync(res.workflowTasks.Select(x => x.TargetId).ToList(), res.workflowTasks.Select(s => s.WorkflowToken).ToList());

                    //var RequestedPaymentss = await _repo.GetAllPurchaseRequisitionNoteAsync();

                    
                    var payments = staffRequestedPaymentsawaiting.Select(d => new ProposalPayment
                    {
                        PaymentTermId = d.PaymentTermId,
                        Completion = d.Completion,
                        Amount = d.Amount,
                        NetAmount = d.NetAmount,
                        Payment = d.Payment,
                        PaymentStatus = d.PaymentStatus,
                        PaymentStatusName = Convert.ToString(d.PaymentStatus),
                        Phase = d.Phase,
                        PhaseName = Convert.ToString((PaymentTermsPhase)d.Phase),
                        ProjectStatusDescription = d.ProjectStatusDescription,
                        Status = d.Status,
                        StatusName = Convert.ToString((JobProgressStatus)d.Status),
                        Comment = d.Comment,
                        AmountPaid = 0,
                        LPOId = d.LPOId,
                        AmountPayable = d.NetAmount,
                        PaymentOutstanding = d.NetAmount,
                        
                    }).ToList();

                    if(payments.Count() > 0)
                    {
                        foreach(var item in payments)
                        {
                            var alreadyPaidPhases = _dataContext.cor_paymentterms.Where(a => a.LPOId == item.LPOId && a.ProposedBy == (int)Proposer.STAFF && a.PaymentStatus == (int)PaymentStatus.Paid).ToList();
                            if (alreadyPaidPhases.Count() > 0)
                            {
                                item.AmountPaid = alreadyPaidPhases.Sum(f => f.NetAmount);
                                item.PaymentOutstanding = (item.AmountPayable - alreadyPaidPhases.Sum(f => f.NetAmount));
                            }
                        }
                    }
                    return new ProposalPaymentRespObj
                    {
                        ProposalPayment = payments,
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage { FriendlyMessage = staffRequestedPaymentsawaiting.Count() > 0 ? "" : "Search Complete : No Payment awaitingt approvals" }
                        }
                    };
                }
                catch (SqlException ex)
                {
                    throw ex;
                }

            }
        }
    }
    
}
