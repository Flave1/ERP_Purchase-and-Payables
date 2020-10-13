using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Puchase_and_payables.DomainObjects.Bid_and_Tender
{
    public class cor_bid_and_tender : GeneralEntity
    {
        [Key]
        public int BidAndTenderId { get; set; }
        public int SupplierId { get; set; }
        public string LPOnumber { get; set; }
        public int PLPOId { get; set; }
        public int RequestingDepartment { get; set; }
        public string DescriptionOfRequest { get; set; }
        public DateTime RequestDate { get; set; }
        public string Suppliernumber { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; } 
        public string Location { get; set; }
        public decimal AmountApproved { get; set; }
        public string ProposalTenderUploadName { get; set; }
        public string ProposalTenderUploadPath { get; set; }
        public string ProposalTenderUploadFullPath { get; set; }
        public string ProposalTenderUploadType { get; set; } 
        public string Extention { get; set; }
        public decimal ProposedAmount { get; set; }
        public DateTime DateSubmitted { get; set; }
        public int DecisionResult { get; set; }
        public int ApprovalStatusId { get; set; }
        public string WorkflowToken { get; set; }
        public string SupplierAddress { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public int PurchaseReqNoteId { get; set; }
        public bool IsRejected { get; set; }
        public string Comment { get; set; }
        public string SelectedSuppliers { get; set; }
        public virtual ICollection<cor_paymentterms> Paymentterms { get; set; }
    }
}
