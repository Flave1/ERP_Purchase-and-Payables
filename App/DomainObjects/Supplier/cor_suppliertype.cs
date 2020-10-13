using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GODP.APIsContinuation.DomainObjects.Supplier
{
    public partial class cor_suppliertype : GeneralEntity
    {
        public cor_suppliertype()
        {
            cor_supplier = new HashSet<cor_supplier>();
        }

        [Key]
        public int SupplierTypeId { get; set; }

        [Required]
        [StringLength(250)]
        public string SupplierTypeName { get; set; }
        public string TaxApplicable { get; set; }

        public int CreditGL { get; set; }
        public int DebitGL { get; set; }

        public virtual ICollection<cor_supplier> cor_supplier { get; set; }
    }
}
