using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GODP.APIsContinuation.DomainObjects.Supplier
{


    public partial class cor_topsupplier : GeneralEntity
    {
        [Key]
        public int TopSupplierId { get; set; }

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

        [StringLength(50)]
        public string ContactPerson { get; set; }

        public int? NoOfStaff { get; set; } 
        public DateTime IncorporationDate { get; set; }
        public int CountryId { get; set; }

        public virtual cor_supplier cor_supplier { get; set; }
    }
}
