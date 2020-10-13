namespace GODP.APIsContinuation.DomainObjects.Supplier
{
    using Puchase_and_payables.Contracts.GeneralExtension;
    using System; 
    using System.ComponentModel.DataAnnotations; 

    public partial class cor_topclient : GeneralEntity
    {
        [Key]
        public int TopClientId { get; set; }

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
        public int CountryId { get; set; }
    }
}
