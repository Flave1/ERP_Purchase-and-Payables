using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
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
 
    public class GetAvailableBidsQueryHandler : IRequestHandler<GetAvailableBidsQuery, BidAndTenderRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly IIdentityServerRequest _serverRequest;
        public GetAvailableBidsQueryHandler(
            IPurchaseService purchaseService,
            IIdentityServerRequest _request)
        {
            _repo = purchaseService;
            _serverRequest = _request;
        }
        public async Task<BidAndTenderRespObj> Handle(GetAvailableBidsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetAllBidAndTender();

            var statusPendingList = result.Where(s => s.ApprovalStatusId == (int)ApprovalStatus.Pending);
            CompanyStructureRespObj _Department = new CompanyStructureRespObj();
            _Department = await _serverRequest.GetAllCompanyStructureAsync();
            return new BidAndTenderRespObj
            {
                BidAndTenders = result.Where(a => a.ApprovalStatusId != (int)ApprovalStatus.Disapproved && (int)ApprovalStatus.Authorised != a.ApprovalStatusId  && a.SupplierId != 0).OrderByDescending(q => q.BidAndTenderId).Select(d => new BidAndTenderObj
                {
                    BidAndTenderId = d.BidAndTenderId,
                    AmountApproved = d.AmountApproved,
                    DateSubmitted = d.DateSubmitted,
                    DecisionResult = d.DecisionResult,
                    DescriptionOfRequest = d?.DescriptionOfRequest,
                    Location = d?.Location,
                    LPOnumber = d?.LPOnumber, 
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
                    ProposalTenderUploadType = d.ProposalTenderUploadType,
                    ProposalTenderUploadPath = d.ProposalTenderUploadPath,
                    ProposalTenderUploadName = d.ProposalTenderUploadName,
                    ProposalTenderUploadFullPath = d.ProposalTenderUploadFullPath,
                    ExpectedDeliveryDate = d.ExpectedDeliveryDate,
                    StatusName = Convert.ToString((ApprovalStatus)d.ApprovalStatusId),
                    RequestingDepartmentName = _Department.companyStructures.FirstOrDefault(e => e.CompanyStructureId == d.RequestingDepartment)?.Name,
                }).ToList() ?? new List<BidAndTenderObj>(),
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
