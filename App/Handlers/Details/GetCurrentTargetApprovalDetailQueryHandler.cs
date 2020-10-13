using GODP.APIsContinuation.Repository.Interface; 
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR; 
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Queries.Details;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Repository.Details;
using Puchase_and_payables.Requests;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Approvals
{ 

    public class GetCurrentTargetApprovalDetailQueryHandler : IRequestHandler<GetCurrentTargetApprovalDetailQuery, ApprovalDetailsRespObj>
    {
        private readonly IWorkflowDetailService _detail;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly ISupplierRepository _supRepo;
        public GetCurrentTargetApprovalDetailQueryHandler(
            IWorkflowDetailService workflowDetailService, 
            IIdentityServerRequest identityServerRequest,
            ISupplierRepository supplierRepository)
        { 
            _detail = workflowDetailService;
            _serverRequest = identityServerRequest;
            _supRepo = supplierRepository;
        }
        public async Task<ApprovalDetailsRespObj> Handle(GetCurrentTargetApprovalDetailQuery request, CancellationToken cancellationToken)
        {
            var list = await _detail.GetApprovalDetailsAsync(request.TargetId, request.WorkflowToken);
            var supplierList = await _supRepo.GetAllSupplierAsync();
            var staff = await _serverRequest.GetAllStaffAsync();
            var response = new ApprovalDetailsRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
              
            var temp = list;
            var previousStaff = temp.GroupBy(d => d.StaffId).Select(d => d.First()).Where(d => d.StatusId == (int)ApprovalStatus.Approved && d.TargetId == request.TargetId && d.WorkflowToken == request.WorkflowToken).ToArray();

            response.AprovalDetails = list.Select(x => new AprovalDetailsObj()
            {
                ApprovalDetailId = x.ApprovalDetailId,
                Comment = x.Comment,
                Date = x.Date,
                StaffId = x.StaffId,
                FirstName = staff.FirstOrDefault(d => d.staffId == x.StaffId)?.firstName,
                LastName = staff.FirstOrDefault(d => d.staffId == x.StaffId)?.lastName,
                StatusId = x.StatusId,
                StatusName = Convert.ToString((ApprovalStatus)x.StatusId),
                SupplierName = supplierList.FirstOrDefault(d => d.SupplierId == x.TargetId)?.Name,
                TargetId = x.TargetId,
                ArrivalDate = x.ArrivalDate,
                WorkflowToken = x.WorkflowToken, }).ToList();
            response.PreviousStaff = previousStaff.Select(p => new PreviousStaff
            {
                StaffId = p.StaffId,
                Name = $"{staff.FirstOrDefault(d => d.staffId == p.StaffId)?.firstName} {staff.FirstOrDefault(d => d.staffId == p.StaffId)?.lastName}",
            }).ToList();
            return response;
        }
    }
}
