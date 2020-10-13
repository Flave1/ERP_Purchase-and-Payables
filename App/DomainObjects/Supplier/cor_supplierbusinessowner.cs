using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GODP.APIsContinuation.DomainObjects.Supplier
{
    public partial class cor_supplierbusinessowner : GeneralEntity
    {
        [Key]
        public int SupplierBusinessOwnerId { get; set; }

        public int SupplierId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(550)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string PhoneNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "image")]
        public byte[] Signature { get; set; } 

        public virtual cor_supplier cor_supplier { get; set; }
    }
}
