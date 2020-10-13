using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.ApprovalRes
{
    public class AprovalDetailsObj
    {
        public int ApprovalDetailId { get; set; }
        public int StatusId { get; set; }
        public int StaffId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int TargetId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SupplierName { get; set; }
        public string StatusName { get; set; }
        public string WorkflowToken { get; set; }
        public DateTime ArrivalDate { get; set; }
    }

    public class PreviousStaff
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
    }
    public class ApprovalDetailsRespObj
    {
        public List<AprovalDetailsObj> AprovalDetails { get; set; }
        public List<PreviousStaff> PreviousStaff { get; set; }
        public List<IndentityServerStaffObj> staff { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class IndentityServerStaffObj
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int JobTitle { get; set; }
        public string Email { get; set; }
    }
}
