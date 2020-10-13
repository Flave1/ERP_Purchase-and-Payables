using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GODP.APIsContinuation.DomainObjects.Supplier
{
    public partial class cor_supplier : GeneralEntity
    {
    
        [Key]
        public int SupplierId { get; set; }

        public int SupplierTypeId { get; set; }
        public string UserId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Passport { get; set; }

        [Required]
        [StringLength(550)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string PhoneNo { get; set; }

        [StringLength(50)]
        public string RegistrationNo { get; set; }

        public int CountryId { get; set; }

        public int ApprovalStatusId { get; set; } 

        //---
        public string Website { get; set; }
        public string TaxIDorVATID { get; set; }
        public string PostalAddress { get; set; }
        public string SupplierNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool HaveWorkPrintPermit { get; set; }
        public int Particulars { get; set; }
        public string WorkflowToken { get; set; }
        //---

        //public virtual cor_approvalstatus cor_approvalstatus { get; set; }

        public virtual cor_suppliertype cor_suppliertype { get; set; }


        public virtual ICollection<cor_supplierauthorization> cor_supplierauthorization { get; set; }


        public virtual ICollection<cor_supplierbusinessowner> cor_supplierbusinessowner { get; set; }


        public virtual ICollection<cor_supplierdocument> cor_supplierdocument { get; set; }


        public virtual ICollection<cor_topsupplier> cor_topsupplier { get; set; }


        //public virtual ICollection<purch_plpo> purch_plpo { get; set; }


       // public virtual ICollection<purch_requisitionnote> purch_requisitionnote { get; set; }
    }
}
