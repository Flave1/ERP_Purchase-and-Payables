using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http; 
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System; 
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class GetApprovedPRNQuery : IRequest<RequisitionNoteRespObj>
    {
        public class GetApprovedPRNQueryHandler : IRequestHandler<GetApprovedPRNQuery, RequisitionNoteRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IHttpContextAccessor _accessor;

            public GetApprovedPRNQueryHandler(
                IPurchaseService Repository, IIdentityServerRequest identityServerRequest)
            {
                _repo = Repository;
                _serverRequest = identityServerRequest;
            }
            public async Task<RequisitionNoteRespObj> Handle(GetApprovedPRNQuery request, CancellationToken cancellationToken)
            {
                var prn = await _repo.GetAllPurchaseRequisitionNoteAsync();
                 
                var user = await _serverRequest.UserDataAsync();

                return new RequisitionNoteRespObj
                {
                    RequisitionNotes = prn.Where(t => t.ApprovalStatusId == (int)ApprovalStatus.Approved).OrderByDescending(r => r.PurchaseReqNoteId).Select(d => new RequisitionNoteObj
                    {
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
                        WorkflowToken = d.WorkflowToken,
                        RequestDate = d.CreatedOn,
                        PRNNumber = d.PRNNumber, 
                        DetailsCount = d.purch_prndetails.Count()
                    }).ToList(),
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = prn.Count() < 1 ? "No PRN awaiting approvals" : null
                        }
                    }
                };
            }
        }
    }
   
}
