using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Puchase_and_payables.DomainObjects.Purchase
{
    public partial class purch_plpodetails : GeneralEntity
    {
        [Key]
        public int PLPODetailsId { get; set; }

        public int SNo { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        public int NoOfItems { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal SubTotal { get; set; }

        public int PLPOId { get; set; } 

        public virtual purch_plpo purch_plpo { get; set; }
    }
}
