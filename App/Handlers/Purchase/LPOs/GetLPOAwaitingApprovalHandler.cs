using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class GetLPOAwaitingApprovalQueryHandler : IRequestHandler<GetLPOAwaitingApprovalQuery, LPORespObj>
    {
        private readonly IPurchaseService _repo; 
        private readonly IIdentityServerRequest _serverRequest; 

        public GetLPOAwaitingApprovalQueryHandler(
            IPurchaseService Repository, 
            IIdentityServerRequest identityServerRequest)
        { 
            _repo = Repository; 
            _serverRequest = identityServerRequest;
        }
         
        public async Task<LPORespObj> Handle(GetLPOAwaitingApprovalQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                if (!result.IsSuccessStatusCode)
                {
                    var data1 = await result.Content.ReadAsStringAsync();
                    var res1 = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data1);
                    return new LPORespObj
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
                    return new LPORespObj
                    {
                        Status = res.Status
                    };
                }

                if (res.workflowTasks.Count() < 1)
                {
                    return new LPORespObj
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
                var staffLPOawaiting = await _repo.GetLPOAwaitingApprovalAsync(res.workflowTasks.Select(x => x.TargetId).ToList(), res.workflowTasks.Select(s => s.WorkflowToken).ToList());

                var lpos = await _repo.GetAllPurchaseRequisitionNoteAsync();
                return new LPORespObj
                {
                    LPOs = staffLPOawaiting?.Select(d => new LPOObj
                    {
                        AmountPayable = d.AmountPayable,
                        ApprovalStatusId = d.ApprovalStatusId,
                        BidAndTenderId = d.BidAndTenderId,
                        DeliveryDate = d.DeliveryDate,
                        Description = d.Description,
                        GrossAmount = d.GrossAmount,
                        JobStatus = d.JobStatus,
                        JobStatusName = Convert.ToString((JobProgressStatus)d.JobStatus),
                        LPONumber = d.LPONumber,
                        Name = d.Name,
                        PLPOId = d.PLPOId,
                        RequestDate = d.RequestDate,
                        SupplierAddress = d.SupplierAddress,
                        SupplierId = d.SupplierIds,
                        SupplierNumber = d.SupplierNumber,
                        Tax = d.Tax,
                        Total = d.Total,
                        WorkflowToken = d.WorkflowToken,
                        WinnerSupplierId = d.WinnerSupplierId,
                        BidAndTender = bids.Where(w => w.BidAndTenderId == d.BidAndTenderId).Select(s => new BidAndTenderObj
                        {
                            BidAndTenderId = s.BidAndTenderId,
                            AmountApproved = s.AmountApproved,
                            ApprovalStatusId = s.ApprovalStatusId,
                            DateSubmitted = s.DateSubmitted,
                            DecisionResult = s.DecisionResult,
                            DecisionReultName = Convert.ToString((DecisionResult)s.DecisionResult),
                            DescriptionOfRequest = s.DescriptionOfRequest,
                            ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                            Location = s.Location,
                            LPOnumber = s.LPOnumber,
                            PLPOId = s.PLPOId,
                            PRNId = s.PLPOId,
                            ProposedAmount = s.ProposedAmount.ToString(),
                            Quantity = s.Quantity,
                            RequestDate = s.RequestDate,
                            RequestingDepartmentName = _Department.companyStructures.FirstOrDefault(e => e.CompanyStructureId == s.RequestingDepartment)?.Name,
                            SupplierAddress = s.SupplierAddress,
                            Suppliernumber = s.Suppliernumber,
                            SupplierName = s.SupplierName,
                            Total = s.Total,  
                        }).ToList(),
                        RequisitionNotes = lpos.Where(w => w.PurchaseReqNoteId == d.PurchaseReqNoteId).Select(q => new RequisitionNoteObj
                        {
                            PurchaseReqNoteId = q.PurchaseReqNoteId,
                            ApprovalStatusId = q.ApprovalStatusId,
                            Comment = q.Comment,
                            DeliveryLocation = q.DeliveryLocation,
                            DepartmentId = q.DepartmentId,
                            Description = q.Description,
                            DocumentNumber = q.DocumentNumber,
                            ExpectedDeliveryDate = q.ExpectedDeliveryDate,
                            IsFundAvailable = q.IsFundAvailable,
                            PRNNumber = q.PRNNumber,
                            RequestBy = q.RequestBy,
                            Total = q.Total,
                            RequestDate = q.CreatedOn,

                        }).ToList(),
                        PaymentTerms = paymentTerms.Where(e => e.BidAndTenderId == d.BidAndTenderId).Select(p => new PaymentTermsObj
                        {
                            BidAndTenderId = p.BidAndTenderId, 
                            Comment = p.Comment,
                            Completion = p.Completion,
                            Amount = p.Amount,
                            NetAmount = p.NetAmount,
                            Payment = p.Payment,
                            PaymentStatus = p.PaymentStatus,
                            PaymentTermId = p.PaymentTermId,
                            Phase = p.Phase,
                            ProjectStatusDescription = p.ProjectStatusDescription,
                            Status = p.Status,
                            ProposedBy = p.ProposedBy,
                            StatusName = Convert.ToString((JobProgressStatus)p.Status),
                            PaymentStatusName = Convert.ToString((PaymentStatus)p.PaymentStatus),
                        }).ToList(),
                    }).ToList() ?? new List<LPOObj>(),
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = staffLPOawaiting.Count() > 0 ? null : "Search Complete! No Record found"
                        }
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
