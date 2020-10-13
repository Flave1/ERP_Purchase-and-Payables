using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Invoice
{
    public class inv_invoice : GeneralEntity
    {
        [Key]
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int LPOId { get; set; }
        public string LPONumber { get; set; }
        public decimal AmountPayable { get; set; }
        public int PaymentTermId { get; set; }
        public string Description { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public int SupplierId { get; set; }
        public string WorkflowToken { get; set; }
        public int ApprovalStatusId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DateInvoiceGenerated { get; set; }
        public int CurrencyId { get; set; }
        public int CreditGl { get; set; }
        public int DebitGl { get; set; }
        public int PaymentBankId { get; set; }  
        public int SupplierBankId { get; set; }
    }
}
