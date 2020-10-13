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

    public class GetPRNAwaitingApprovalQueryHandler : IRequestHandler<GetPRNAwaitingApprovalQuery, RequisitionNoteRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _accesor;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly ISupplierRepository _supRepo;

        public GetPRNAwaitingApprovalQueryHandler(
            IPurchaseService Repository,
            IMapper mapper,
            ISupplierRepository supplierRepository,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerRequest identityServerRequest)
        {
            _mapper = mapper;
            _supRepo = supplierRepository;
            _repo = Repository;
            _factory = httpClientFactory;
            _accesor = httpContextAccessor;
            _serverRequest = identityServerRequest;
        }
        private string GetStatusName(int Status)
        {
            var name = (ApprovalStatus)Status;
            return name.ToString();
        }

        private IEnumerable<int> SplitedSupplierIds(string Ids)
        {
            var splitedIds = Ids.Split(',').Select(int.Parse);
            return splitedIds;
        }
        public async Task<RequisitionNoteRespObj> Handle(GetPRNAwaitingApprovalQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var suppliers = await _supRepo.GetListOfApprovedSuppliersAsync(); 
                var prn = await _repo.GetPurchaseRequisitionNoteAsync(request.PrnId);
                var prnDetails = await _repo.GetPrnDetailsByPrnId(request.PrnId);
                var listOfSupplierIds = prn.purch_prndetails.Select(e => e.SuggestedSupplierId);
                var joinedAllSelectedSupplierId = string.Join(',', listOfSupplierIds.Distinct().Select(int.Parse));
                var selectedSuppliers = suppliers.Where(d => SplitedSupplierIds(joinedAllSelectedSupplierId).Contains(d.SupplierId));

                var prnList = new List<RequisitionNoteObj>();
                if( prn != null)
                {
                    var item = new RequisitionNoteObj
                    {
                        ApprovalStatusId = prn.ApprovalStatusId,
                        Comment = prn.Comment,
                        DeliveryLocation = prn.DeliveryLocation,
                        DepartmentId = prn.DepartmentId,
                        Description = prn.Description,
                        DocumentNumber = prn.DocumentNumber,
                        ExpectedDeliveryDate = prn.ExpectedDeliveryDate,
                        IsFundAvailable = prn.IsFundAvailable,
                        PurchaseReqNoteId = prn.PurchaseReqNoteId,
                        RequestBy = prn.RequestBy,
                        StatusName = GetStatusName(prn.ApprovalStatusId),
                        Total = prn.Total,
                        DetailsCount = prn.purch_prndetails.Count(),
                        WorkflowToken = prn.WorkflowToken,
                        RequestDate = prn.CreatedOn,
                        PRNDetails = prnDetails.Select(
                            s => new PRNDetailsObj()
                            {
                                Description = s.Description,
                                IsBudgeted = s.IsBudgeted,
                                PRNDetailsId = s.PRNDetailsId,
                                PurchaseReqNoteId = s.PurchaseReqNoteId,
                                Quantity = s.Quantity,
                                SubTotal = s.SubTotal,
                                UnitPrice = s.UnitPrice,
                                SuggestedSupplierId = SplitedSupplierIds(s.SuggestedSupplierId),
                                Suppliers = string.Join(" , ", suppliers.Where(e => SplitedSupplierIds(s.SuggestedSupplierId).Contains(e.SupplierId)).Select(q => q.Name).ToList()),
                            }).ToList()
                    };
                    prnList.Add(item);
                }

                return new RequisitionNoteRespObj
                {
                    RequisitionNotes = prnList,
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = prn == null ? "No PRN detail awaiting approvals" : null
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
