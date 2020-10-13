using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Supplier
{
    public class cor_identification : GeneralEntity
    {
        [Key]
        public int IdentificationId { get; set; }
        public int SupplierId { get; set; }
        public bool IsCorporate { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? IncorporationDate { get; set; }
        public int BusinessType { get; set; }
        public string OtherBusinessType { get; set; }

        public int Identification  { get; set; } 
        public string Identification_Number { get; set; }
        public DateTime? Expiry_Date { get; set; }
        public int Nationality { get; set; }
        public bool HaveWorkPermit { get; set; }
    }
}
