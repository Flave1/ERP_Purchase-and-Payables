using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Purchase
{
    public class RequisitionNoteObj
    {
        public int PurchaseReqNoteId { get; set; }
         
        public string RequestBy { get; set; }
        public string PRNNumber { get; set; }

        public int DepartmentId { get; set; }
         
        public string DocumentNumber { get; set; }
         
        public string Description { get; set; }
        public string Comment { get; set; }

        public bool? IsFundAvailable { get; set; }
         
        public string DeliveryLocation { get; set; }

        public decimal? Total { get; set; }
         
        public DateTime? RequestDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int ApprovalStatusId { get; set; }
        public string StatusName { get; set; }
        public int DetailsCount { get; set; }
        public string WorkflowToken { get; set; }
        public string DepartmentName { get; set; }
        public List<PRNDetailsObj> PRNDetails { get; set; }
    }

    public class RequisitionNoteRespObj
    {
        public List<RequisitionNoteObj> RequisitionNotes { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class RequisitionNoteRegRespObj
    {
        public int PurchaseReqNoteId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class PRNDetailsObj
    {
        public int PRNDetailsId { get; set; } 
        public string Description { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Comment { get; set; }
        public decimal SubTotal { get; set; } 
        public int PurchaseReqNoteId { get; set; }
        public IEnumerable<int> SuggestedSupplierId { get; set; } 
        public bool? IsBudgeted { get; set; }
        public string  Suppliers { get; set; }
    } 
    public class TempSuppliers
    {
        public string Name { get; set; } 
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
    }

    public class LPOObj
    {
        public int PLPOId { get; set; } 
        public string Name { get; set; }  
        public string SupplierId { get; set; } 
        public decimal Tax { get; set; } 
        public decimal Total { get; set; } 
        public DateTime DeliveryDate { get; set; }
        public string LPONumber { get; set; }
        public string Description { get; set; }
        public int ApprovalStatusId { get; set; } 
        public string SupplierNumber { get; set; }
        public string SupplierAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal AmountPayable { get; set; }
        public int JobStatus { get; set; }
        public string JobStatusName { get; set; }
        public int BidAndTenderId { get; set; }
        public string WorkflowToken { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int WinnerSupplierId { get; set; }
        public int ServicetermsId { get; set; }
        public List<TaxObj> Taxes { get; set; }
        public List<BidAndTenderObj>  BidAndTender { get; set; }
        public List<RequisitionNoteObj> RequisitionNotes { get; set; }
        public List<PaymentTermsObj> PaymentTerms { get; set; }
    }
    public class TaxObj
    {
        public int TaxID { get; set; }
        public string TaxName { get; set; }
        public bool ShouldExcept { get; set; }
        public double Percentage { get; set; }
        public string Type { get; set; }
        public int SubGL { get; set; }
    }
    public class LPORegRespObj
    {
        public int PLPOId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class LPORespObj
    {
        public List<LPOObj> LPOs { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class BidAndTenderObj
    {
        public int BidAndTenderId { get; set; }
        public string LPOnumber { get; set; }
        public int PLPOId { get; set; }
        public int SupplierId { get; set; }
        public int RequestingDepartment { get; set; }
        public string DescriptionOfRequest { get; set; }
        public DateTime RequestDate { get; set; }
        public string Suppliernumber { get; set; }
        public string SupplierName { get; set; }
        public string Location { get; set; }
        public decimal AmountApproved { get; set; }
        public string ProposalTenderUploadName { get; set; }
        public string ProposalTenderUploadPath { get; set; }
        public string ProposalTenderUploadFullPath { get; set; }
        public string ProposalTenderUploadType { get; set; }
        public string ProposedAmount { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime DateSubmitted { get; set; }
        public int DecisionResult { get; set; }
        public int ApprovalStatusId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; } 
        public string DecisionReultName { get; set; }
        public string WorkflowToken { get; set; }
        public string RequestingDepartmentName { get; set; }
        public string SupplierAddress { get; set; }
        public int PRNId { get; set; }
        public string Comment { get; set; }
        public string StatusName { get; set; }
        public List<RequisitionNoteObj> RequisitionNotes { get; set; }
        public List<PaymentTermsObj> PaymentTerms { get; set; }
    }

    public class BidAndTenderRespObj
    {
        public List<BidAndTenderObj> BidAndTenders { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class BidAndTenderRegRespObj
    {
        public int BidAndTenderId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class InvoiceObj
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber  { get; set; }
        public int Payment { get; set; }
        public string LPONumber { get; set; }
        public string Supplier { get; set; }
        public int SupplierId { get; set; }
        public string Location { get; set; }
        public decimal Amount { get; set; }
        public string DescriptionOfRequest { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public decimal AmountPayable { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal PaymentOutstanding { get; set; }
        public DateTime RequestDate { get; set; }
        public string Workflowtoken { get; set; }
        public int PaymentTermId { get; set; }
        public string BankName { get; set; }
        public int LpoId { get; set; }
        public int PaymentBankId { get; set; }
        public int SupplierBankId { get; set; }  
        public int Phase { get; set; }
        public decimal GrossAmount { get; set; }
        public double Tax { get; set; }
        public List<PaymentTermsObj> PaymentTerms { get; set; }
    }

    public class InvoiceRespObj
    {
        public List<InvoiceObj> Invoices { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class PaymentApprovalRespObj
    {
        public List<InvoiceObj> PaymentApprovals { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
