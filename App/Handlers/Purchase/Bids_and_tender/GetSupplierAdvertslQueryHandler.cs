using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth;
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
    public class GetBidAndTenderBySupplierEmailQueryHandler : IRequestHandler<GetBidAndTenderQuery, BidAndTenderRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataContext _dataContext;
        public GetBidAndTenderBySupplierEmailQueryHandler(
            IPurchaseService purchaseService,
            IIdentityServerRequest serverRequest,
            IHttpContextAccessor accessor,
            DataContext dataContext,
            UserManager<ApplicationUser> userManager)
        { 
            _repo = purchaseService;
            _dataContext = dataContext;
            _accessor = accessor;
            _userManager = userManager;
            _serverRequest = serverRequest;
        }
        public async Task<BidAndTenderRespObj> Handle(GetBidAndTenderQuery request, CancellationToken cancellationToken)
        {
            var response = new BidAndTenderRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            var userid = _accessor.HttpContext.User?.FindFirst(a => a.Type == "userId")?.Value; 
            var user = await _userManager.FindByIdAsync(userid);

            var otherBids = await _dataContext.cor_bid_and_tender.Where(a => a.SupplierId == 0 && a.ApprovalStatusId == (int)ApprovalStatus.Awaiting).ToListAsync();
            var supDetail = await _dataContext.cor_supplier.FirstOrDefaultAsync(a => a.Email.Trim().ToLower() == user.Email.Trim().ToLower());
            var result = await _repo.GetAllSupplierBidAndTender(user.Email);
            CompanyStructureRespObj _Department = new CompanyStructureRespObj();
            var resp = new List<BidAndTenderObj>();

            _Department = await _serverRequest.GetAllCompanyStructureAsync();
            var domainList = new List<cor_bid_and_tender>();

            domainList.AddRange(result);
           
            if (otherBids.Count() > 0)
            {
                var nonSupplierBids = otherBids.Where(q => !q.SelectedSuppliers.Split(',').Select(int.Parse).ToList().Contains(supDetail.SupplierId)).ToList();
                if (nonSupplierBids.Count() > 0)
                { 
                    foreach (var er in nonSupplierBids)
                    {
                        er.SupplierName = supDetail.Name;
                        er.Suppliernumber = supDetail.SupplierNumber;
                        er.SupplierId = supDetail.SupplierId;  
                    } 
                }
                domainList.AddRange(nonSupplierBids);
            }
            response.BidAndTenders = domainList?.OrderByDescending(a => a.BidAndTenderId).Where(r => r.ApprovalStatusId == (int)ApprovalStatus.Awaiting)
                .Select(d => new BidAndTenderObj
                {
                    BidAndTenderId = d.BidAndTenderId,
                    AmountApproved = d.AmountApproved,
                    DateSubmitted = d.DateSubmitted,
                    DecisionResult = d.DecisionResult,
                    DescriptionOfRequest = d?.DescriptionOfRequest,
                    Location = d?.Location,
                    LPOnumber = d?.LPOnumber, 
                    ExpectedDeliveryDate = d.ExpectedDeliveryDate, 
                    RequestDate = d.RequestDate,
                    RequestingDepartment = d.RequestingDepartment,
                    SupplierName = d?.SupplierName,
                    Suppliernumber = d?.Suppliernumber, 
                    Quantity = d.Quantity,
                    Total = d.Total,
                    ApprovalStatusId = d.ApprovalStatusId,
                    SupplierId = d.SupplierId,
                    WorkflowToken = d.WorkflowToken,
                    Comment = d.Comment,
                    SupplierAddress = d.SupplierAddress,
                    RequestingDepartmentName = d.RequestingDepartment > 0? _Department.companyStructures.FirstOrDefault(e => e.CompanyStructureId == d.RequestingDepartment)?.Name : string.Empty,
                    PLPOId = d.PLPOId,
                    PRNId = d.PurchaseReqNoteId, 
                    
                }).ToList() ?? new List<BidAndTenderObj>();
             
            if (response.BidAndTenders.Count() > 0)
            {
                var biddenITem = _dataContext.cor_bid_and_tender.Where(q => q.SupplierId == supDetail.SupplierId && response.BidAndTenders.Select(s => s.PLPOId).Contains(q.PLPOId) && !string.IsNullOrEmpty(q.SelectedSuppliers)).ToList();
                if (biddenITem.Count() > 0)
                {
                   var lrespn = response.BidAndTenders.Except(response.BidAndTenders.Where(a => biddenITem.Select(w => w.PLPOId).Contains(a.PLPOId)).ToList());
                    response.BidAndTenders = lrespn.ToList();
                }
            }
            
            return response;
        }
        
    }
}
