using GODP.APIsContinuation.DomainObjects.Supplier;
using Puchase_and_payables.Contracts.GeneralExtension;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Puchase_and_payables.DomainObjects.Purchase
{
    public partial class purch_plpo : GeneralEntity
    {
        public purch_plpo()
        {
            purch_plpodetails = new HashSet<purch_plpodetails>();
        }

        [Key]
        public int PLPOId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        [Required]
        [StringLength(550)]
        public string Address { get; set; }
        public string SupplierIds { get; set; }

        [Column(TypeName = "money")]
        public decimal Tax { get; set; }

        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        [Column(TypeName = "date")]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [StringLength(50)]
        public string LPONumber { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public int ApprovalStatusId { get; set; }
        //.....
        public string SupplierNumber { get; set; }
        public string SupplierAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal AmountPayable { get; set; }
        public int JobStatus { get; set; }
        public bool BidComplete { get; set; } = false;
        public int BidAndTenderId { get; set; }
        public int WinnerSupplierId { get; set; } 
        public string WorkflowToken { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int PurchaseReqNoteId { get; set; }
        public string Taxes { get; set; }
        public int DebitGl { get; set; }
        public int ServiceTerm { get; set; }
        public DateTime? DateCompleted { get; set; }

        public virtual cor_supplier cor_supplier { get; set; }
        public virtual ICollection<purch_plpodetails> purch_plpodetails { get; set; }
    }
}
