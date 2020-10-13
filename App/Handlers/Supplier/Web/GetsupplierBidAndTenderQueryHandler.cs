using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class GetPreviousBidAndTenderQuery : IRequest<BidAndTenderRespObj>
    {
        public GetPreviousBidAndTenderQuery() { }
        public string SupplierEmail { get; set; }
        public GetPreviousBidAndTenderQuery(string supplierEmail) { SupplierEmail = supplierEmail; }
        public class GetPreviousBidAndTenderQueryHandler : IRequestHandler<GetPreviousBidAndTenderQuery, BidAndTenderRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly IIdentityServerRequest _serverRequest; 
            public readonly UserManager<ApplicationUser> _userManager; 
            public GetPreviousBidAndTenderQueryHandler(
                IPurchaseService purchaseService,
                IIdentityServerRequest serverRequest,
                DataContext dataContext,
                UserManager<ApplicationUser> userManager)
            {
                _repo = purchaseService; 
                _serverRequest = serverRequest;
                _userManager = userManager;
            }
            public async Task<BidAndTenderRespObj> Handle(GetPreviousBidAndTenderQuery request, CancellationToken cancellationToken)
            {
                var result = await _repo.GetAllPrevSupplierBidAndTender(request.SupplierEmail);
                var response = new BidAndTenderRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
                CompanyStructureRespObj _Department = new CompanyStructureRespObj();
                if (result.Count() > 0)
                {
                    _Department = await _serverRequest.GetAllCompanyStructureAsync();
                } 

                response.BidAndTenders = result?.Where(a => a.ApprovalStatusId == (int)ApprovalStatus.Approved || a.ApprovalStatusId == (int)ApprovalStatus.Pending)
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
                    }).ToList() ?? new List<BidAndTenderObj>();
                response.Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = result.Count() > 0 ? null : "Search Complete! No Record found"
                    }
                };
                return response;
            }

        }
    }
    
}
