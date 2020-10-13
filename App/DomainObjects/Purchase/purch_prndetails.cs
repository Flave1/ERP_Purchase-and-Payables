using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Puchase_and_payables.DomainObjects.Purchase
{
    public partial class purch_prndetails : GeneralEntity
    {
        [Key]
        public int PRNDetailsId { get; set; } 
        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        public int Quantity { get; set; }
         
        public decimal UnitPrice { get; set; }
         
        public decimal SubTotal { get; set; }

        public int PurchaseReqNoteId { get; set; }
        public string SuggestedSupplierId { get; set; }

        public bool? IsBudgeted { get; set; }
        public string LPONumber { get; set; } 
        public string Comment { get; set; }

        public virtual purch_requisitionnote purch_requisitionnote { get; set; }
    }
}
