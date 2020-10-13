using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Approvals;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Repository.Details;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
   
	public class LPOStaffApprovalCommandHandler : IRequestHandler<LPOStaffApprovalCommand, StaffApprovalRegRespObj>
	{
		 
		private readonly IPurchaseService _repo;
		private readonly ISupplierRepository _supRepo;
		private readonly IWorkflowDetailService _detailService;
		private readonly IHttpContextAccessor _accessor;
		private readonly IIdentityServerRequest _serverRequest;
		private readonly IFinanceServerRequest _financeServer;
		private readonly DataContext _dataContext;
		private StaffApprovalRegRespObj response = new StaffApprovalRegRespObj();
		public LPOStaffApprovalCommandHandler(
			ISupplierRepository supplierRepository,
			IHttpContextAccessor httpContextAccessor,
			IPurchaseService purchaseService,
			IWorkflowDetailService detailService,
			IIdentityServerRequest serverRequest,
			IFinanceServerRequest financeServer,
			DataContext dataContext)
		{
			_repo = purchaseService;
			_supRepo = supplierRepository;
			_accessor = httpContextAccessor;
			_serverRequest = serverRequest; 
			_dataContext = dataContext;
			_detailService = detailService;
			_financeServer = financeServer;
		}
	 
		public async Task<StaffApprovalRegRespObj> Handle(LPOStaffApprovalCommand request, CancellationToken cancellationToken)
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

				var currentLPO = await _repo.GetLPOsAsync(request.TargetId);
		 
				List<cor_paymentterms> paymentTerms = await _repo.GetPaymenttermsAsync();

				var detail = BuildApprovalDetailObject(request, currentLPO, user.StaffId);

				var req = new IndentityServerApprovalCommand
				{
					ApprovalComment = request.ApprovalComment,
					ApprovalStatus = request.ApprovalStatus,
					TargetId = request.TargetId,
					WorkflowToken = currentLPO.WorkflowToken,
					ReferredStaffId = request.ReferredStaffId
				};

				using (var _trans = await _dataContext.Database.BeginTransactionAsync())
				{
					try
					{
						var result = await _serverRequest.StaffApprovalRequestAsync(req);

						if (!result.IsSuccessStatusCode)
						{
							response.Status.Message.FriendlyMessage = result.ReasonPhrase.ToString();
							return apiResponse;
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
							currentLPO.ApprovalStatusId = (int)ApprovalStatus.Processing;  
							await _repo.AddUpdateLPOAsync(currentLPO);

							await _trans.CommitAsync(); 
							apiResponse.Status = response.Status;
							apiResponse.Status.IsSuccessful = true;
							return apiResponse;
						}
						if (response.ResponseId == (int)ApprovalStatus.Revert)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentLPO.ApprovalStatusId = (int)ApprovalStatus.Revert;
							await _repo.AddUpdateLPOAsync(currentLPO);

							await _trans.CommitAsync(); 
							apiResponse.Status = response.Status;
							return apiResponse; 
						}
						if (response.ResponseId == (int)ApprovalStatus.Approved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentLPO.ApprovalStatusId = (int)ApprovalStatus.Approved;
							await _repo.AddUpdateLPOAsync(currentLPO);
							await _repo.ShareTaxToPhasesIthereIsAsync(currentLPO);
							await _repo.RemoveLostBidsAndProposals(currentLPO);
							await _trans.CommitAsync();
							apiResponse.Status = response.Status;
							apiResponse.Status.Message.FriendlyMessage = $"Final Approval </br> for this LPO {currentLPO.LPONumber}";
							return apiResponse; 
						}
						if (response.ResponseId == (int)ApprovalStatus.Disapproved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentLPO.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
							await _repo.AddUpdateLPOAsync(currentLPO);
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
  
		private cor_approvaldetail BuildApprovalDetailObject(LPOStaffApprovalCommand request,  purch_plpo currentItem, int staffId)
		{
			var approvalDeatil = new cor_approvaldetail(); 
			var previousDetail = _detailService.GetApprovalDetailsAsync(request.TargetId, currentItem.WorkflowToken).Result;
			approvalDeatil.ArrivalDate = currentItem.RequestDate;

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
