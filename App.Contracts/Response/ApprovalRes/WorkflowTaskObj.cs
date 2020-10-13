using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.ApprovalRes
{

    public class WorkflowTaskRespObj
    {
        public List<WorkflowTaskObj> workflowTasks { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class WorkflowTaskObj
    {
        public int TargetId { get; set; }
        public int OperationId { get; set; }
        public int WorkflowId { get; set; }
        public string StaffEmail { get; set; }
        public string WorkflowToken { get; set; }
    }
}
