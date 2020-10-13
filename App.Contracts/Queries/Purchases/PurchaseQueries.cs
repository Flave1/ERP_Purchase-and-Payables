using MediatR;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Queries.Purchases
{
    public class GetAllPRNAwaitingApprovalQuery : IRequest<RequisitionNoteRespObj> { }
	public class GetAllPRNQuery : IRequest<RequisitionNoteRespObj> { }

	public class GetPRNAwaitingApprovalQuery : IRequest<RequisitionNoteRespObj>
	{
		public GetPRNAwaitingApprovalQuery() { }
		public int PrnId { get; set; }
		public GetPRNAwaitingApprovalQuery(int prnId)
		{
			PrnId = prnId;
		}
	}

	

	public class GetAllLPOQuery : IRequest<LPORespObj> { }
	public class GetBidAndTenderQuery : IRequest<BidAndTenderRespObj>
	{
		public GetBidAndTenderQuery() { }
		public string SupplierEmail { get; set; }
		public GetBidAndTenderQuery(string supplierEmail)
		{
			SupplierEmail = supplierEmail;
		}
	}

	public class GetSupplierAdvertsNotBiddenForQuery : IRequest<BidAndTenderRespObj> { }
	public class GetBidAndTenderByIdQuery : IRequest<BidAndTenderRespObj>
	{
		public GetBidAndTenderByIdQuery() { }
		public int BidAndTenderID { get; set; }
		public GetBidAndTenderByIdQuery(int bidAndTenderID)
		{
			BidAndTenderID = bidAndTenderID;
		}
	}

	public class GetBidAndTenderAwaitingApprovalQuery : IRequest<BidAndTenderRespObj> { }

	public class GetAvailableBidsQuery : IRequest<BidAndTenderRespObj> { }

	public class GetListInvoicesQuery : IRequest<InvoiceRespObj> { }
	public class GetInvoiceDetailQuery : IRequest<InvoiceRespObj>
	{
		public GetInvoiceDetailQuery() { }
		public int InvoiceId { get; set; }
		public GetInvoiceDetailQuery(int invoiceId) { InvoiceId = invoiceId; }
	}

	public class GetLPOAwaitingApprovalQuery : IRequest<LPORespObj> { } 
}
