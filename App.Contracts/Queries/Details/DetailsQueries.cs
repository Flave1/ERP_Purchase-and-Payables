using MediatR;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Queries.Details
{
    public class GetCurrentTargetApprovalDetailQuery : IRequest<ApprovalDetailsRespObj>
    {
        public GetCurrentTargetApprovalDetailQuery() { }
        public int TargetId { get; set; }
        public string WorkflowToken { get; set; }
        public GetCurrentTargetApprovalDetailQuery(int targetId, string workflwToken)
        {
            TargetId = targetId;
            WorkflowToken = workflwToken;
        }
    }

}
