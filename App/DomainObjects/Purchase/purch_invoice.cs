using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Purchase
{
    public class purch_invoice : GeneralEntity
    {
        [Key]
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int LPOId { get; set; }
        public string LPONumber { get; set; }
        public decimal AmountPayable { get; set; }
        public string PaymentTermIds { get; set; }
        public string Description { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Address { get; set; }
        public decimal GrossAmount { get; set; }
        public int SupplierId { get; set; }
    }
}
