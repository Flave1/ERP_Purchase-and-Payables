    using Puchase_and_payables.Contracts.GeneralExtension; 
using System.ComponentModel.DataAnnotations; 

namespace Puchase_and_payables.DomainObjects.Supplier
{
    public class cor_taxsetup : GeneralEntity
    {
        [Key]
        public int TaxSetupId { get; set; }
        public string TaxName { get; set; }
        public double Percentage { get; set; }
        public string Type { get; set; }
        public int SubGL { get; set; }
    }
}
