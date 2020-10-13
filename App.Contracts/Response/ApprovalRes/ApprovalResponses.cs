using GOSLibraries.GOS_API_Response; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.ApprovalRes
{
      

    public class GoForApprovalRespObj
    {
        public int SupplierId { get; set; }
        public bool HasWorkflowAccess { get; set; }
        public bool EnableWorkflow { get; set; }
        public bool ApprovalProcessStarted { get; set; }
        public APIResponseStatus Status { get; set; }
    }


    public class GoForApprovalRequest
    {
        public int StaffId { get; set; }
        public int CompanyId { get; set; }
        public int StatusId { get; set; }
        public List<int> TargetId { get; set; }
        public string Comment { get; set; }
        public int OperationId { get; set; }
        public bool DeferredExecution { get; set; }
        public int WorkflowId { get; set; }
        public bool ExternalInitialization { get; set; }
        public bool EmailNotification { get; set; }
        public int WorkflowTaskId { get; set; }
        public int ApprovalStatus { get; set; }
    }

    public class StaffApprovalRegRespObj
    {
        public int ResponseId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

   
}
