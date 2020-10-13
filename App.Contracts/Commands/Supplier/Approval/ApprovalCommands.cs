using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Purchase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Commands.Supplier.Approval
{
    public class SupplierStaffApprovalCommand : IRequest<StaffApprovalRegRespObj>
    {
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public int ReferredStaffId { get; set; }
        public int TargetId { get; set; } 
    }

    public class PRNStaffApprovalCommand : IRequest<StaffApprovalRegRespObj>
    {
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public int ReferredStaffId { get; set; }
        public int TargetId { get; set; } 
    }

    public class BidandTenderStaffApprovalCommand : IRequest<StaffApprovalRegRespObj>
    {
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public int ReferredStaffId { get; set; }
        public int TargetId { get; set; } 
    }

    public class IndentityServerApprovalCommand
    {
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public int TargetId { get; set; }
        public int ReferredStaffId { get; set; }
        public string WorkflowToken { get; set; }
    }

    public class GothroughApprovalCommand : IRequest<SupplierRegRespObj>
    {
        public int SupplierId { get; set; }
    }

    public class LPOStaffApprovalCommand : IRequest<StaffApprovalRegRespObj>
    {
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public int ReferredStaffId { get; set; }
        public int TargetId { get; set; }
    }

    
    public class PaymentRequestStaffApprovalCommand : IRequest<StaffApprovalRegRespObj>
    {
        public int ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public int ReferredStaffId { get; set; }
        public int TargetId { get; set; }
    }
}
