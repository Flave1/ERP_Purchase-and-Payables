using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Commands.Purchase
{
    public class AddUpdateRequisitionNoteCommand : IRequest<RequisitionNoteRegRespObj>
    {
        public int PurchaseReqNoteId { get; set; }
        public string RequestBy { get; set; }
        public int DepartmentId { get; set; }
        public string DocumentNumber { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public bool? IsFundAvailable { get; set; }
        public string DeliveryLocation { get; set; }
        public decimal Total { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int ApprovalStatusId { get; set; }
        public List<PRNDetailsObj> PRNDetails { get; set; }
    }

  
    public class DeleteRequisitionNoteCommand : IRequest<DeleteItemReqObj>
    {
        public List<int> PurchaseReqNoteIds { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class SendSupplierBidAndTenderToApprovalCommand : IRequest<BidAndTenderRegRespObj>
    {
        public int BidAndTenderId { get; set; }
    }
    public class SendPRNToApprovalCommand : IRequest<RequisitionNoteRegRespObj>
    {
        public int PurchaseReqNoteId { get; set; }
    } 

    public class AddUpdateBidAndTenderByStaffCommand : IRequest<BidAndTenderRegRespObj>
    {
        public int BidAndTenderId { get; set; }
        public int SupplierId { get; set; }
        public string LPONumber { get; set; }
        public int RequestingDepartment { get; set; }
        public string DescriptionOfRequest { get; set; }
        public DateTime RequestDate { get; set; }
        public string Suppliernumber { get; set; }
        public string SupplierName { get; set; }
        public string Location { get; set; }
        public decimal AmountApproved { get; set; }
        public IFormFile ProposalTenderUpload { get; set; }
        public decimal ProposedAmount { get; set; } 
        public int DecisionResult { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        public int CompanyId { get; set; }
        public int PurchaseReqNoteId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public List<PaymentTermsObj> Paymentterms { get; set; }
    }
    public class AddUpdateBidAndTenderCommand : IRequest<BidAndTenderRegRespObj>
    {
        public int BidAndTenderId { get; set; }
        public int SupplierId { get; set; }
        public string LPONumber { get; set; }
        public int RequestingDepartment { get; set; }
        public string DescriptionOfRequest { get; set; }
        public DateTime RequestDate { get; set; }
        public string Suppliernumber { get; set; }
        public string SupplierName { get; set; }
        public string Location { get; set; }
        public decimal AmountApproved { get; set; }
        public IFormFile ProposalTenderUpload { get; set; }
        public decimal ProposedAmount { get; set; }
        //public DateTime DateSubmitted { get; set; }
        public int DecisionResult { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        public int CompanyId { get; set; }
        public int PurchaseReqNoteId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public List<PaymentTermsObj> Paymentterms { get; set; }
    }
    //public class UpdatePaymentTermsCommand : IRequest<PaymentTermsRegRespObj>
    //{
    //    public int PaymentTermId { get; set; }
    //    public int BidAndTenderId { get; set; }
    //    public int Phase { get; set; }
    //    public double Payment { get; set; }
    //    public string ProjectStatusDescription { get; set; }
    //    public double Completion { get; set; }
    //    public string Comment { get; set; }
    //    public byte[] CcompletionCertificate { get; set; }
    //    public int Status { get; set; }
    //    public decimal GrossAmount { get; set; }
    //    public decimal NetAmount { get; set; }
    //    public int PaymentStatus { get; set; }
    //    public string StatusName { get; set; }
    //}
    public enum Proposer 
    { 
        STAFF = 1,
        SUPPLIER = 2
    }
    public class UpdateTerms
    { 
        public int PaymentTermId { get; set; }
        public int Phase { get; set; }
        public double Payment { get; set; }
        public string ProjectStatusDescription { get; set; }
        public double Completion { get; set; }
        public string Comment { get; set; } 
        public decimal GrossAmount { get; set; }
        public decimal Amount { get; set; }
    }
     
}
