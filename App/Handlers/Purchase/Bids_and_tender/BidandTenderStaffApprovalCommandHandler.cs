using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response; 
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
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

	public class BidandTenderStaffApprovalCommandHandler : IRequestHandler<BidandTenderStaffApprovalCommand, StaffApprovalRegRespObj>
	{

		private readonly IPurchaseService _repo;
		private readonly IHttpContextAccessor _accessor;
		private readonly IIdentityServerRequest _serverRequest;
		private readonly IWorkflowDetailService _detailService;
		private readonly ISupplierRepository _supRepo;
		private readonly DataContext _dataContext; 
		private StaffApprovalRegRespObj response = new StaffApprovalRegRespObj();
		public BidandTenderStaffApprovalCommandHandler(
			IHttpContextAccessor httpContextAccessor, 
			IPurchaseService purchaseService,
			IIdentityServerRequest serverRequest,
			IWorkflowDetailService detailService,
			DataContext dataContext,
			ISupplierRepository supplierRepository)
		{
			_repo = purchaseService; 
			_accessor = httpContextAccessor;
			_serverRequest = serverRequest;
			_detailService = detailService;
			_dataContext = dataContext;
			_supRepo = supplierRepository;
		}

		public async Task<StaffApprovalRegRespObj> Handle(BidandTenderStaffApprovalCommand request, CancellationToken cancellationToken)
		{
			try
			{

				if (request.ApprovalStatus == (int)ApprovalStatus.Revert && request.ReferredStaffId < 1)
				{
					return new StaffApprovalRegRespObj
					{
						Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please select staff to revert to" } }
					};
				}

				var currentUserId = _accessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
				var user = await _serverRequest.UserDataAsync();

				var currentBid = await _repo.GetBidAndTender(request.TargetId);
				if(!Validation(currentBid).Status.IsSuccessful)
				{
					return Validation(currentBid);
				}

			
				var _ThisBidLPO = await _repo.GetLPOByNumberAsync(currentBid.LPOnumber);

				if (_ThisBidLPO.WinnerSupplierId > 0 && request.ApprovalStatus != (int)ApprovalStatus.Approved)
				{
					return new StaffApprovalRegRespObj
					{
						Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $"Supplier already selected for this LPO {currentBid.LPOnumber}" } }
					};
				}

				IEnumerable<cor_paymentterms> paymentTerms = await _repo.GetPaymenttermsAsync();

				var detail = BuildApprovalDetailObject(request, currentBid, user.StaffId);

				var req = new IndentityServerApprovalCommand
				{
					ApprovalComment = request.ApprovalComment,
					ApprovalStatus = request.ApprovalStatus,
					TargetId = request.TargetId,
					WorkflowToken = currentBid.WorkflowToken,
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
							return new StaffApprovalRegRespObj
							{
								Status = response.Status
							};
						}

						if (response.ResponseId == (int)ApprovalStatus.Processing)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentBid.ApprovalStatusId = (int)ApprovalStatus.Processing;
							currentBid.DecisionResult = (int)DecisionResult.Non_Applicable;

							var thisBidPaymentTerms = paymentTerms.Where(d => d.BidAndTenderId == currentBid.BidAndTenderId).ToList();
							currentBid.Paymentterms = thisBidPaymentTerms;

							await _repo.AddUpdateBidAndTender(currentBid);
							await _trans.CommitAsync();
							return new StaffApprovalRegRespObj
							{
								ResponseId = (int)ApprovalStatus.Processing,
								Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
							};
						}
						if (response.ResponseId == (int)ApprovalStatus.Revert)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentBid.ApprovalStatusId = (int)ApprovalStatus.Revert;
							currentBid.DecisionResult = (int)DecisionResult.Non_Applicable;


						 
							await _repo.AddUpdateBidAndTender(currentBid);
							await _trans.CommitAsync();
							return new StaffApprovalRegRespObj
							{
								ResponseId = (int)ApprovalStatus.Revert,
								Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
							};
						}
						if (response.ResponseId == (int)ApprovalStatus.Approved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							currentBid.ApprovalStatusId = (int)ApprovalStatus.Approved;
							currentBid.DecisionResult = (int)DecisionResult.Win;
							currentBid.PLPOId = _ThisBidLPO.PLPOId;
							var thisBidPaymentTerms = paymentTerms.Where(d => d.BidAndTenderId == currentBid.BidAndTenderId).ToList();
							currentBid.Paymentterms = thisBidPaymentTerms;
							await _repo.SendEmailToSuppliersSelectedAsync(currentBid.SupplierId, currentBid.DescriptionOfRequest, _ThisBidLPO.PLPOId);
							await _repo.AddUpdateBidAndTender(currentBid);
							var terms = paymentTerms.Where(q => q.BidAndTenderId == currentBid.BidAndTenderId).ToList();
							foreach(var term in terms)
							{
								term.LPOId = currentBid.PLPOId;
								term.CompanyId = currentBid.CompanyId;
								await _repo.AddUpdatePaymentTermsAsync(term);
							}
							var otherBids = _dataContext.cor_bid_and_tender.Where(q => q.LPOnumber.Trim().ToLower() == currentBid.LPOnumber.Trim().ToLower() && q.BidAndTenderId != currentBid.BidAndTenderId).ToList();
							if (otherBids.Count() > 0)
							{
								foreach(var otherbid in otherBids)
								{
									otherbid.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
									otherbid.DecisionResult = (int)DecisionResult.Lost;
									otherbid.Paymentterms = _dataContext.cor_paymentterms.Where(q => q.BidAndTenderId == otherbid.BidAndTenderId).ToList();
									await _repo.AddUpdateBidAndTender(otherbid);
								}
							}
							var thisBidLpo = _repo.BuildThisBidLPO(_ThisBidLPO, currentBid); 
							await _repo.AddUpdateLPOAsync(thisBidLpo);

							await _trans.CommitAsync();
							return new StaffApprovalRegRespObj
							{
								ResponseId = (int)ApprovalStatus.Approved,
								Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = $"Final Approval </br> Successfully selected Supplier for  {currentBid.LPOnumber}" } 
							}
							};
						}
						if (response.ResponseId == (int)ApprovalStatus.Disapproved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							_ThisBidLPO.JobStatus = _ThisBidLPO.JobStatus;
							currentBid.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
							currentBid.DecisionResult = (int)DecisionResult.Lost;
							await _repo.AddUpdateBidAndTender(currentBid);
							await _trans.CommitAsync();
							return new StaffApprovalRegRespObj
							{
								ResponseId = (int)ApprovalStatus.Disapproved,
								Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
							};
						}
					}
					catch (Exception ex)
					{
						await _trans.RollbackAsync();
						throw new Exception("Error Occurerd", new Exception($"{ex.Message}"));
					}
					finally { await _trans.DisposeAsync(); }

				}

				return new StaffApprovalRegRespObj
				{
					ResponseId = detail.ApprovalDetailId,
					Status = response.Status
				};
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		 
		private StaffApprovalRegRespObj Validation(cor_bid_and_tender item)
		{
			//if(item.DecisionResult == (int)DecisionResult.Lost)
			//{
			//	return new StaffApprovalRegRespObj
			//	s
			//		Status = new APIResponseStatus
			//		{
			//			IsSuccessful = false,
			//			Message = new APIResponseMessage
			//			{
			//				FriendlyMessage = "Already lost the bid"
			//			}
			//		}
			//	};
			//}

			if (item.DecisionResult == (int)DecisionResult.Win)
			{
				return new StaffApprovalRegRespObj
				{
					Status = new APIResponseStatus
					{
						IsSuccessful = false,
						Message = new APIResponseMessage
						{
							FriendlyMessage = "Bid Already Approved"
						}
					}
				};
			}
			return new StaffApprovalRegRespObj
			{
				Status = new APIResponseStatus { IsSuccessful = true, }
			};
		}

		private cor_approvaldetail BuildApprovalDetailObject(BidandTenderStaffApprovalCommand request, cor_bid_and_tender currentItem, int staffId)
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
			approvalDeatil.ReferredStaffId = request.ReferredStaffId;
			approvalDeatil.StaffId = staffId;
			approvalDeatil.WorkflowToken = currentItem.WorkflowToken;
			return approvalDeatil;
		}
	}
}
