using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Bid_and_Tender
{
    public class cor_paymentterms : GeneralEntity
    {
        [Key]
        public int PaymentTermId { get; set; }
        public int LPOId { get; set; }
        public int Phase { get; set; }
        public double Payment { get; set; }
        public string ProjectStatusDescription { get; set; }
        public double Completion { get; set; }
        public string Comment { get; set; }
        public int BidAndTenderId { get; set; }
        public string CcompletionCertificateName { get; set; }
        public string CcompletionCertificatePath { get; set; }
        public string CcompletionCertificateFullPath { get; set; }
        public string CcompletionCertificateType { get; set; }
        public int Status { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int PaymentStatus { get; set; }
        public string WorkflowToken { get; set; }
        public int ApprovalStatusId { get; set; }
        public int ProposedBy { get; set; }
        public int BankGl { get; set; }
        public int SubGl { get; set; }
        public string Taxes { get; set; }
        public double TaxPercent { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal Amount { get; set; } 
        public bool InvoiceGenerated { get; set; }
        public cor_bid_and_tender BidAndTender { get; set; }
    }
}
