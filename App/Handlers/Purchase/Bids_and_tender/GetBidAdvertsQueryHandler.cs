using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class GetBidAdvertsQuery : IRequest<BidAndTenderRespObj>
    {
        public class GetBidAdvertsQueryHandler : IRequestHandler<GetBidAdvertsQuery, BidAndTenderRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IHttpContextAccessor _accessor;
            public GetBidAdvertsQueryHandler(
                IPurchaseService purchaseService,
                IIdentityServerRequest serverRequest,
                IHttpContextAccessor accessor)
            {
                _repo = purchaseService;
                _accessor = accessor;
                _serverRequest = serverRequest;
            }
            public async Task<BidAndTenderRespObj> Handle(GetBidAdvertsQuery request, CancellationToken cancellationToken)
            {  

                var paymentTerms = await _repo.GetPaymenttermsAsync();
                return new BidAndTenderRespObj
                {
                    BidAndTenders = result?.Where(r => r.ApprovalStatusId == (int)ApprovalStatus.Awaiting)
                    .Select(d => new BidAndTenderObj
                    {
                        BidAndTenderId = d.BidAndTenderId,
                        AmountApproved = d.AmountApproved,
                        DateSubmitted = d.DateSubmitted,
                        DecisionResult = d.DecisionResult,
                        DescriptionOfRequest = d?.DescriptionOfRequest,
                        Location = d?.Location,
                        LPOnumber = d?.LPOnumber,
                        ProposalTenderUploadType = d.ProposalTenderUploadType,
                        ProposalTenderUploadPath = d.ProposalTenderUploadPath,
                        ProposalTenderUploadName = d.ProposalTenderUploadName,
                        ProposalTenderUploadFullPath = d.ProposalTenderUploadFullPath,
                        ExpectedDeliveryDate = d.ExpectedDeliveryDate,
                        ProposedAmount = d.ProposedAmount.ToString(),
                        RequestDate = d.RequestDate,
                        RequestingDepartment = d.RequestingDepartment,
                        SupplierName = d?.SupplierName,
                        Suppliernumber = d?.Suppliernumber,
                        DecisionReultName = Convert.ToString((DecisionResult)d.DecisionResult),
                        Quantity = d.Quantity,
                        Total = d.Total,
                        ApprovalStatusId = d.ApprovalStatusId,
                        SupplierId = d.SupplierId,
                        WorkflowToken = d.WorkflowToken,
                        SupplierAddress = d.SupplierAddress,
                        RequestingDepartmentName = _Department.companyStructures.FirstOrDefault(e => e.CompanyStructureId == d.RequestingDepartment)?.Name,
                        PLPOId = d.PLPOId,
                        PRNId = d.PurchaseReqNoteId,
                        StatusName = Convert.ToString((ApprovalStatus)d.ApprovalStatusId),
                        PaymentTerms = paymentTerms.Where(e => e.BidAdvertsId == d.BidAdvertsId
                        && e.ProposedBy == (int)Proposer.SUPPLIER).Select(q => new PaymentTermsObj
                        {
                            BidAdvertsId = q.BidAdvertsId,
                            Comment = q.Comment,
                            Completion = q.Completion,
                            Amount = q.Amount,
                            NetAmount = q.NetAmount,
                            Payment = q.Payment,
                            PaymentStatus = q.PaymentStatus,
                            Phase = q.Phase,
                            PaymentTermId = q.PaymentTermId,
                            ProjectStatusDescription = q.ProjectStatusDescription,
                            Status = q.Status,
                        }).ToList(),
                    }).ToList() ?? new List<BidAdvertsObj>(),
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = result.Count() > 0 ? null : "Search Complete! No Record found"
                        }
                    }
                };
            }
        }
    }
    
}
