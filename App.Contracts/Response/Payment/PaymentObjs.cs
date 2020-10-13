using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Contracts.Response.Purchase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Payment
{
    //public class InvoiceObj
    //{
    //    public int InvoiceId { get; set; }
    //    public string LPONumber { get; set; }
    //    public string InvoiceNumber { get; set; } 
    //    public string Supplier { get; set; }
    //    public string Location { get; set; }
    //    public string DescriptionOfRequest { get; set; }
    //    public DateTime ExpectedDeliveryDate { get; set; }
    //    public decimal AmountPayable { get; set; }
    //    public decimal AmountPaid { get; set; }
    //    public decimal PaymentOutstanding { get; set; }
    //}

    public class InvoiceRegObj
    {
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

        public DateTime RequestDate { get; set; }
        public int CurrencyId { get; set; }
        public int SubGLId { get; set; }
        public int BankGL { get; set; }
    }
    //public class InvoiceRespObj
    //{
    //    public List<InvoiceObj> Invoices { get; set; }
    //    public APIResponseStatus Status { get; set; }
    //}

    public class InvoiceRegRespObj
    {
        public int InvoiceId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class InvoiceDetailObj
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string LPONumber { get; set; }
        public string Supplier { get; set; }
        public List<ProposalPayment> Payments { get; set; }
    }

    public class PaymentTermsObj
    {
        public int PaymentTermId { get; set; }
        public int BidAndTenderId { get; set; }
        public int Phase { get; set; }
        public double Payment { get; set; }
        public int ProposedBy { get; set; }
        public string ProjectStatusDescription { get; set; }
        public double Completion { get; set; }
        public string Comment { get; set; }
        public byte[] CcompletionCertificate { get; set; }
        public int Status { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public int PaymentStatus { get; set; }
        public string PaymentStatusName { get; set; }
        public string StatusName { get; set; }
        public string PhaseName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal PaymentOutstanding { get; set; }
        public decimal AmountPayable { get; set; }
        public int LPOId { get; set; }
        public double PhaseTax { get; set; }
    }

    public class PaymentTermsRegRespObj
    {
        public int PaymentTermId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
     
    public class ProposalPayment
    {
        public int PaymentTermId { get; set; }
        public int BidAndTenderId { get; set; }
        public int Phase { get; set; }
        public double Payment { get; set; }
        public int ProposedBy { get; set; }
        public string ProjectStatusDescription { get; set; }
        public double Completion { get; set; }
        public string Comment { get; set; }
        public byte[] CcompletionCertificate { get; set; }
        public int Status { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public int PaymentStatus { get; set; }
        public string PaymentStatusName { get; set; }
        public string StatusName { get; set; }
        public string PhaseName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal PaymentOutstanding { get; set; }
        public decimal AmountPayable { get; set; }
        public int LPOId { get; set; }
    }
    public class InvoiceDetailRespObj
    {
        public List<InvoiceDetailObj> InvoiceDetails { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class ProposalPaymentRespObj
    {
        public List<ProposalPayment> ProposalPayment { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
