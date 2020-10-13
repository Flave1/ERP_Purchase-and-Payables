using AutoMapper;
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
    public class GetAllPRNAwaitingApprovalQueryHandler : IRequestHandler<GetAllPRNAwaitingApprovalQuery, RequisitionNoteRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _accesor;
        private readonly IIdentityServerRequest _serverRequest;

        public GetAllPRNAwaitingApprovalQueryHandler(
            IPurchaseService Repository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerRequest identityServerRequest)
        {
            _mapper = mapper;
            _repo = Repository;
            _factory = httpClientFactory;
            _accesor = httpContextAccessor;
            _serverRequest = identityServerRequest;
        }
       
        public async Task<RequisitionNoteRespObj> Handle(GetAllPRNAwaitingApprovalQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = new RequisitionNoteRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };

                var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                if (!result.IsSuccessStatusCode)
                {
                    apiResponse.Status.Message.FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}";
                    return apiResponse;
                }
                var data = await result.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data);

                if (res == null)
                {
                    apiResponse.Status = res.Status;
                    return apiResponse;
                }

                if (res.workflowTasks.Count() < 1)
                {
                    apiResponse.Status.IsSuccessful = true;
                    apiResponse.Status.Message.FriendlyMessage = "No Pending Approval";
                    return apiResponse;
                }

                var _Department = await _serverRequest.GetAllCompanyStructureAsync();

                var pendingTaskIds = res.workflowTasks.Select(x => x.TargetId).ToList();
                var pendingTaskTokens = res.workflowTasks.Select(s => s.WorkflowToken).ToList();
                var prn = await _repo.GetPRNAwaitingApprovalAsync(pendingTaskIds, pendingTaskTokens);

                apiResponse.RequisitionNotes = prn.Select(d => new RequisitionNoteObj {
                    ApprovalStatusId = d.ApprovalStatusId,
                    Comment = d.Comment,
                    DeliveryLocation = d.DeliveryLocation,
                    DepartmentId = d.DepartmentId,
                    Description = d.Description,
                    DocumentNumber = d.DocumentNumber,
                    ExpectedDeliveryDate = d.ExpectedDeliveryDate,
                    IsFundAvailable = d.IsFundAvailable,
                    PurchaseReqNoteId = d.PurchaseReqNoteId,
                    RequestBy = d.RequestBy,
                    StatusName = Convert.ToString((ApprovalStatus)d.ApprovalStatusId),
                    Total = d.Total,
                    RequestDate = d.CreatedOn,
                    WorkflowToken = d.WorkflowToken,
                    PRNNumber = d.PRNNumber, 
                    DepartmentName = _Department.companyStructures.FirstOrDefault(c => c.CompanyStructureId == d.DepartmentId)?.Name,
                    DetailsCount = d.purch_prndetails.Count()
                }).ToList();

                apiResponse.Status.IsSuccessful = true;
                apiResponse.Status.Message.FriendlyMessage = prn.Count() < 1 ? "No PRN awaiting approvals" : null;
                return apiResponse;
            }
            catch (SqlException ex)
            {
                throw ex;
            } 
        }
    }
}
