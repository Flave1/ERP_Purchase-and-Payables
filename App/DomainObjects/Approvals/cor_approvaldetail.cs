using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.ComponentModel.DataAnnotations;

namespace Puchase_and_payables.DomainObjects.Approvals
{
    public class cor_approvaldetail : GeneralEntity
    {
        [Key]
        public int ApprovalDetailId { get; set; }
        public int StatusId { get; set; }
        public int StaffId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int TargetId { get; set; }
        public int ReferredStaffId { get; set; }
        public string WorkflowToken { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime ResponseDate { get; set; }
    }
}
