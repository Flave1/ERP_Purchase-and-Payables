using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Approvals;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
using Puchase_and_payables.Repository.Details;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
   
	public class PaymentRequestStaffApprovalCommandHandler : IRequestHandler<PaymentRequestStaffApprovalCommand, StaffApprovalRegRespObj>
	{
		 
		private readonly IPurchaseService _repo;
		private readonly ISupplierRepository _supRepo;
		private readonly IWorkflowDetailService _detailService;
		private readonly IHttpContextAccessor _accessor;
		private readonly IIdentityServerRequest _serverRequest;
		private readonly IFinanceServerRequest _financeServer;
		private readonly DataContext _dataContext;
		private readonly IInvoiceService _invoice;
		private StaffApprovalRegRespObj response = new StaffApprovalRegRespObj();
		public PaymentRequestStaffApprovalCommandHandler(
			ISupplierRepository supplierRepository,
			IHttpContextAccessor httpContextAccessor,
			IPurchaseService purchaseService,
			IWorkflowDetailService detailService,
			IIdentityServerRequest serverRequest,
			IFinanceServerRequest financeServer,
			IInvoiceService invoiceService,
			DataContext dataContext)
		{
			_repo = purchaseService;
			_supRepo = supplierRepository;
			_accessor = httpContextAccessor;
			_serverRequest = serverRequest; 
			_dataContext = dataContext;
			_detailService = detailService;
			_invoice = invoiceService;
			_financeServer = financeServer;
		} 
		public async Task<StaffApprovalRegRespObj> Handle(PaymentRequestStaffApprovalCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var apiResponse = new StaffApprovalRegRespObj { Status = new APIResponseStatus {IsSuccessful = false, Message = new APIResponseMessage() } };
				if (request.ApprovalStatus == (int)ApprovalStatus.Revert && request.ReferredStaffId < 1)
				{
					return new StaffApprovalRegRespObj
					{
						Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please select staff to revert to" } }
					};
				}

				var currentUserId = _accessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
				var user = await _serverRequest.UserDataAsync();

				var currentInvoice = await _invoice.GetInvoiceAsync(request.TargetId);
		   
				var detail = BuildApprovalDetailObject(request, currentInvoice, user.StaffId);

				var req = new IndentityServerApprovalCommand
				{
					ApprovalComment = request.ApprovalComment,
					ApprovalStatus = request.ApprovalStatus,
					TargetId = request.TargetId,
					WorkflowToken = currentInvoice.WorkflowToken,
					ReferredStaffId = request.ReferredStaffId
				};

				using (var _trans = await _dataContext.Database.BeginTransactionAsync())
				{
					try
					{
						var result = await _serverRequest.StaffApprovalRequestAsync(req);

						if (!result.IsSuccessStatusCode)
						{
							return new StaffApprovalRegRespObj
							{
								Status = new APIResponseStatus
								{
									IsSuccessful = false,
									Message = new APIResponseMessage { FriendlyMessage = result.ReasonPhrase }
								}
							};
						}

						var stringData = await result.Content.ReadAsStringAsync();
						response = JsonConvert.DeserializeObject<StaffApprovalRegRespObj>(stringData);

						if (!response.Status.IsSuccessful)
						{ 
							apiResponse.Status = response.Status;
							return apiResponse;
						}
						if (response.ResponseId == (int)ApprovalStatus.Processing)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentInvoice.ApprovalStatusId = (int)ApprovalStatus.Processing;  
							await _invoice.CreateUpdateInvoiceAsync(currentInvoice);

							await _trans.CommitAsync(); 
							apiResponse.Status = response.Status;
							apiResponse.Status.IsSuccessful = true;
							return apiResponse;
						}
						if (response.ResponseId == (int)ApprovalStatus.Revert)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentInvoice.ApprovalStatusId = (int)ApprovalStatus.Revert;
							await _invoice.CreateUpdateInvoiceAsync(currentInvoice);

							await _trans.CommitAsync(); 
							apiResponse.Status = response.Status;
							return apiResponse; 
						}
						if (response.ResponseId == (int)ApprovalStatus.Approved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentInvoice.ApprovalStatusId = (int)ApprovalStatus.Approved;

							var thisInvoicePhase = await _repo.GetSinglePaymenttermAsync(currentInvoice.PaymentTermId);
							thisInvoicePhase.PaymentStatus = (int)PaymentStatus.Paid;
							thisInvoicePhase.CompletionDate = DateTime.Now; 
							thisInvoicePhase.ApprovalStatusId = (int)ApprovalStatus.Approved;
							await _repo.AddUpdatePaymentTermsAsync(thisInvoicePhase);
							currentInvoice.AmountPaid = currentInvoice.Amount;
							currentInvoice.ApprovalStatusId = (int)ApprovalStatus.Approved;
							await _invoice.CreateUpdateInvoiceAsync(currentInvoice);
							var paymentResp = await _invoice.TransferPaymentAsync(currentInvoice);
							if (!paymentResp.Status.IsSuccessful)
							{
								await _trans.CommitAsync(); 
								apiResponse.Status.Message.FriendlyMessage = $"Final Approval Successful <br/>But payment not made 'Reason (s): {paymentResp.Status.Message.FriendlyMessage}";
								return apiResponse;
							}
							await _repo.SendEmailToSupplierDetailingPaymentAsync(currentInvoice, thisInvoicePhase.Phase);
							await _trans.CommitAsync();
							apiResponse.Status = response.Status;
							apiResponse.Status.Message.FriendlyMessage = $"Final Approval Successful";
							return apiResponse; 
						}
						if (response.ResponseId == (int)ApprovalStatus.Disapproved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentInvoice.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
 
							await _invoice.CreateUpdateInvoiceAsync(currentInvoice);
							await _trans.CommitAsync();
							apiResponse.Status = response.Status;
							return apiResponse;
						}
					}
					catch (Exception ex)
					{
						await _trans.RollbackAsync();
						throw ex;
					}
					finally { await _trans.DisposeAsync(); }

				}
				apiResponse.ResponseId = detail.ApprovalDetailId;
				apiResponse.Status = response.Status;
				return apiResponse; 
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private cor_approvaldetail BuildApprovalDetailObject(PaymentRequestStaffApprovalCommand request, inv_invoice currentItem, int staffId)
		{
			var approvalDeatil = new cor_approvaldetail();
			var previousDetail = _detailService.GetApprovalDetailsAsync(request.TargetId, currentItem.WorkflowToken).Result;
			approvalDeatil.ArrivalDate = currentItem.CreatedOn ?? DateTime.Now;

			if (previousDetail.Count() > 0)
				approvalDeatil.ArrivalDate = previousDetail.OrderByDescending(s => s.ApprovalDetailId).FirstOrDefault().Date;

			approvalDeatil.Comment = request.ApprovalComment;
			approvalDeatil.Date = DateTime.Today;
			approvalDeatil.StatusId = request.ApprovalStatus;
			approvalDeatil.TargetId = request.TargetId;
			approvalDeatil.StaffId = staffId;
			approvalDeatil.ReferredStaffId = request.ReferredStaffId;
			approvalDeatil.WorkflowToken = currentItem.WorkflowToken;
			return approvalDeatil;
		}
		
	}
}
