using FluentValidation;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Requests;

namespace Puchase_and_payables.Validators.Approvals
{
    public class StaffApprovalCommandVal : AbstractValidator<PRNStaffApprovalCommand>
    {
        public StaffApprovalCommandVal()
        {
            RuleFor(d => d.ApprovalComment).NotEmpty();
            RuleFor(d => d.ApprovalStatus).NotEmpty();
            RuleFor(d => d.TargetId).NotEmpty();
        }
    }

    public class SupplierStaffApprovalCommandVal : AbstractValidator<SupplierStaffApprovalCommand>
    {
        public SupplierStaffApprovalCommandVal()
        {
            RuleFor(d => d.ApprovalComment).NotEmpty();
            RuleFor(d => d.ApprovalStatus).NotEmpty();
            RuleFor(d => d.TargetId).NotEmpty();
        }
    }
    
    public class BidandTenderStaffApprovalCommandVal : AbstractValidator<BidandTenderStaffApprovalCommand>
    {
        public BidandTenderStaffApprovalCommandVal()
        {
            RuleFor(d => d.ApprovalComment).NotEmpty();
            RuleFor(d => d.ApprovalStatus).NotEmpty();
            RuleFor(d => d.TargetId).NotEmpty();
        }
    }

    public class LPOStaffApprovalCommandVal : AbstractValidator<LPOStaffApprovalCommand>
    {
        public LPOStaffApprovalCommandVal()
        {
            RuleFor(d => d.ApprovalComment).NotEmpty();
            RuleFor(d => d.ApprovalStatus).NotEmpty();
            RuleFor(d => d.TargetId).NotEmpty();
        }
    }
}
