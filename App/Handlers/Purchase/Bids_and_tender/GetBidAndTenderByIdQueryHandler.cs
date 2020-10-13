using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data; 
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{

    public class GetBidAndTenderByIdQueryHandler : IRequestHandler<GetBidAndTenderByIdQuery, BidAndTenderRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly IIdentityServerRequest _serverRequest; 
        public GetBidAndTenderByIdQueryHandler(IPurchaseService purchaseService, IIdentityServerRequest serverRequest, DataContext dataContext)
        {
            _repo = purchaseService; 
            _serverRequest = serverRequest;
        }
        public async Task<BidAndTenderRespObj> Handle(GetBidAndTenderByIdQuery request, CancellationToken cancellationToken)
        {
            CompanyStructureRespObj _Department = new CompanyStructureRespObj();
            _Department = await _serverRequest.GetAllCompanyStructureAsync();

            var d = await _repo.GetBidAndTender(request.BidAndTenderID);
            var prn = await _repo.GetAllPurchaseRequisitionNoteAsync();
            var itemList = new List<BidAndTenderObj>(); 
            if(d != null)
            {
                var item = new BidAndTenderObj
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
                    Quantity = d.Quantity,
                    Total = d.Total,
                    PLPOId = d.PLPOId,
                    Comment = d.Comment,
                    SupplierId = d.SupplierId,
                    WorkflowToken = d.WorkflowToken,
                    RequisitionNotes = prn.Where(a => a.PurchaseReqNoteId == d.PurchaseReqNoteId).Select(v => new RequisitionNoteObj
                    {
                        ApprovalStatusId = v.ApprovalStatusId,
                        Comment = v.Comment,
                        DeliveryLocation = v.DeliveryLocation,
                        DepartmentId = v.DepartmentId,
                        Description = v.Description,
                        DocumentNumber = v.DocumentNumber,
                        ExpectedDeliveryDate = v.ExpectedDeliveryDate,
                        IsFundAvailable = v.IsFundAvailable,
                        PurchaseReqNoteId = v.PurchaseReqNoteId,
                        RequestBy = v.RequestBy,
                        StatusName = Convert.ToString((ApprovalStatus)v.ApprovalStatusId),
                        Total = v.Total,
                        RequestDate = v.CreatedOn,
                        WorkflowToken = v.WorkflowToken,
                        PRNNumber = v.PRNNumber, 
                        
                        DetailsCount = v.purch_prndetails.Count(),
                    }).ToList(),
                RequestingDepartmentName = _Department.companyStructures.FirstOrDefault(e => e.CompanyStructureId == d.RequestingDepartment)?.Name,
                    PaymentTerms = d.Paymentterms.Select(b => new PaymentTermsObj {
                        BidAndTenderId = b.BidAndTenderId, 
                        Comment = b.Comment,
                        Completion = b.Completion,
                        Amount = b.Amount,
                        NetAmount = b.NetAmount,
                        Payment = b.Payment,
                        PaymentStatus = b.PaymentStatus,
                        Phase = b.Phase,
                        PaymentTermId = b.PaymentTermId,
                        Status = b.Status,
                        
                        ProposedBy = b.ProposedBy,
                        ProjectStatusDescription = b.ProjectStatusDescription,
                    }).ToList(),
                };
                itemList.Add(item);
            }

            return new BidAndTenderRespObj
            {
                BidAndTenders = itemList,
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = itemList.Count() > 0 ? null : "Search Complete! No Record found"
                    }
                }
            };
        }
    }

}
