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
using Puchase_and_payables.Data;
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
     

    public class GetBidAndTenderAwaitingApprovalQueryHandler : IRequestHandler<GetBidAndTenderAwaitingApprovalQuery, BidAndTenderRespObj>
    {
        private readonly IPurchaseService _repo; 
        private readonly IIdentityServerRequest _serverRequest;
        private readonly DataContext _dataContext;

        public GetBidAndTenderAwaitingApprovalQueryHandler(
            IPurchaseService Repository, 
            IIdentityServerRequest identityServerRequest,
            DataContext  dataContext)
        { 
            _repo = Repository; 
            _serverRequest = identityServerRequest;
            _dataContext = dataContext;
        }
         
        public async Task<BidAndTenderRespObj> Handle(GetBidAndTenderAwaitingApprovalQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = new BidAndTenderRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };

                CompanyStructureRespObj _Department = new CompanyStructureRespObj();
                _Department = await _serverRequest.GetAllCompanyStructureAsync();

                var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                if (!result.IsSuccessStatusCode)
                {
                    var data1 = await result.Content.ReadAsStringAsync();
                    var res1 = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data1);
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}";
                    return response; 
                }

                var data = await result.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data);

                if (res == null)
                {
                    response.Status = res.Status;
                    return response; 
                }

                if (res.workflowTasks.Count() < 1)
                {
                    response.Status.IsSuccessful = true;
                    response.Status.Message.FriendlyMessage = "No Pending Approval";
                    return response; 
                }
                var staffBidawaiting = await _repo.GetBidAndTenderAwaitingApprovalAsync(res.workflowTasks.Select(x => x.TargetId).ToList(), res.workflowTasks.Select(s => s.WorkflowToken).ToList());
                var paymentTerms = await _repo.GetPaymenttermsAsync();
                var requisitions = await _repo.GetAllPurchaseRequisitionNoteAsync();

                response.BidAndTenders = staffBidawaiting?.Where(w => w.ApprovalStatusId != (int)ApprovalStatus.Disapproved).Select(d => new BidAndTenderObj
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
                    ProposedAmount = d.ProposedAmount.ToString(),
                    RequestDate = d.RequestDate,
                    RequestingDepartment = d.RequestingDepartment,
                    SupplierName = d?.SupplierName,
                    Suppliernumber = d?.Suppliernumber,
                    DecisionReultName = Convert.ToString((DecisionResult)d.DecisionResult),
                    Quantity = d.Quantity,
                    SupplierId = d.SupplierId,
                    PLPOId = d.PLPOId,
                    Total = d.Total,
                    ApprovalStatusId = d.ApprovalStatusId,
                    WorkflowToken = d.WorkflowToken,
                    ExpectedDeliveryDate = d.ExpectedDeliveryDate,
                    SupplierAddress = d.SupplierAddress, 
                    PRNId = d.PurchaseReqNoteId,
                    RequestingDepartmentName = _Department.companyStructures.FirstOrDefault(e => e.CompanyStructureId == d.RequestingDepartment)?.Name,
                    RequisitionNotes = _dataContext.purch_requisitionnote.Where(q => q.PurchaseReqNoteId == d.PurchaseReqNoteId).Select(w => new RequisitionNoteObj { 
                        PurchaseReqNoteId = w.PurchaseReqNoteId,
                        ApprovalStatusId = w.ApprovalStatusId,
                        Comment = w.Comment,
                        DeliveryLocation = w.DeliveryLocation,
                        DepartmentId = w.DepartmentId, 
                        Description = w.Description,
                        DocumentNumber = w.DocumentNumber,
                        ExpectedDeliveryDate = w.ExpectedDeliveryDate,
                        IsFundAvailable = w.IsFundAvailable,
                        PRNNumber = w.PRNNumber, 
                        Total = w.Total,
                        RequestDate = w.CreatedOn,
                        RequestBy = w.RequestBy,
                        StatusName = Convert.ToString((ApprovalStatus)d.ApprovalStatusId), 
                    }).ToList(),
                    PaymentTerms = paymentTerms.Where(s => s.BidAndTenderId == d.BidAndTenderId).Select(a => new PaymentTermsObj
                    {
                        BidAndTenderId = a.BidAndTenderId, 
                        Comment = a.Comment,
                        Completion = a.Completion,
                        Amount = a.Amount,
                        NetAmount = a.NetAmount,
                        Payment = a.Payment,
                        PaymentStatus = a.PaymentStatus,
                        PaymentStatusName = Convert.ToString((PaymentStatus)a.PaymentStatus),
                        PaymentTermId = a.PaymentTermId,
                        Phase = a.Phase,
                        ProjectStatusDescription = a.ProjectStatusDescription,
                        Status = a.Status,
                        ProposedBy = a.ProposedBy,
                        
                        StatusName = Convert.ToString((JobProgressStatus)a.Status), 
                    }).ToList(),
                }).ToList() ?? new List<BidAndTenderObj>();

                response.Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = staffBidawaiting.Count() > 0 ? null : "Search Complete! No Record found"
                    }
                };


                return response;
            }
            catch (SqlException ex)
            {
                throw ex;
            } 
        }
    }
}
