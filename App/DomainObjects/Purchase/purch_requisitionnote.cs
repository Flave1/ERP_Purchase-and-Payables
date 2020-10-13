using GODP.APIsContinuation.DomainObjects.Supplier;
using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Puchase_and_payables.DomainObjects.Purchase
{
    public partial class purch_requisitionnote : GeneralEntity
    {
        public purch_requisitionnote()
        {
            purch_prndetails = new HashSet<purch_prndetails>();
        }

        [Key]
        public int PurchaseReqNoteId { get; set; }

        [Required]
        [StringLength(255)]
        public string RequestBy { get; set; }
         
        public string PRNNumber { get; set; }

        public int DepartmentId { get; set; }

        [StringLength(50)]
        public string DocumentNumber { get; set; }

        [Required] 
        public string Description { get; set; }
        public string Comment { get; set; }
         
        public bool? IsFundAvailable { get; set; }

        [StringLength(1000)]
        public string DeliveryLocation { get; set; }

        public int StaffId { get; set; }
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        public int ApprovalStatusId { get; set; }
        public string WorkflowToken { get; set; }
        [NotMapped]
        public virtual cor_supplier cor_supplier { get; set; }
        [NotMapped]
        public virtual ICollection<purch_prndetails> purch_prndetails { get; set; }
    }
}
