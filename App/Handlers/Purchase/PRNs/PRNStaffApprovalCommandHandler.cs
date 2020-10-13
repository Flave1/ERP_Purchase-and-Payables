using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_MAIL_BOX;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Approvals;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.Helper.Extensions;
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

	public class PRNStaffApprovalCommandHandler : IRequestHandler<PRNStaffApprovalCommand, StaffApprovalRegRespObj>
	{
		 
		private readonly IPurchaseService _repo;
		private readonly IHttpContextAccessor _accessor;
		private readonly IIdentityServerRequest _serverRequest;
		private readonly IWorkflowDetailService _detailService;
		private readonly ISupplierRepository _supRepo;
		private readonly DataContext _dataContext;
		private StaffApprovalRegRespObj response = new StaffApprovalRegRespObj();
		public PRNStaffApprovalCommandHandler(
			IHttpContextAccessor httpContextAccessor,  
			IPurchaseService purchaseService, 
			IIdentityServerRequest serverRequest,
			IWorkflowDetailService detailService,
			ISupplierRepository supplierRepository,
			DataContext dataContext)
		{
			_repo = purchaseService; 
			_accessor = httpContextAccessor;
			_serverRequest = serverRequest;
			_detailService = detailService;
			_dataContext = dataContext;
			_supRepo = supplierRepository;
		}

		public async Task<StaffApprovalRegRespObj> Handle(PRNStaffApprovalCommand request, CancellationToken cancellationToken)
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
				var allSuppliers = await _dataContext.cor_supplier.ToListAsync();
				var currentUserId = _accessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
				var user = await _serverRequest.UserDataAsync();

				var prn = await _repo.GetPurchaseRequisitionNoteAsync(request.TargetId);

				var detail = BuildApprovalDetailObject(request, prn, user.StaffId);

				var req = new IndentityServerApprovalCommand
				{
					ApprovalComment = request.ApprovalComment,
					ApprovalStatus = request.ApprovalStatus,
					TargetId = request.TargetId,
					WorkflowToken = prn.WorkflowToken,
					ReferredStaffId = request.ReferredStaffId
				};

				using(var _trans = await _dataContext.Database.BeginTransactionAsync())
				{
					try
					{
						var result = await _serverRequest.StaffApprovalRequestAsync(req);

						if (!result.IsSuccessStatusCode)
						{
							var stringData2 = await result.Content.ReadAsStringAsync();
							response = JsonConvert.DeserializeObject<StaffApprovalRegRespObj>(stringData2);
							return new StaffApprovalRegRespObj
							{
								Status = response.Status
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
							prn.ApprovalStatusId = (int)ApprovalStatus.Processing;
							await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
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
							prn.ApprovalStatusId = (int)ApprovalStatus.Revert;
							await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
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
							prn.ApprovalStatusId = (int)ApprovalStatus.Approved;
							var prnDetails = await _repo.GetPrnDetailsByPrnId(prn.PurchaseReqNoteId);
							EmailMessageObj email = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>() };

							foreach (var item in prnDetails)
							{ 
								item.LPONumber = _repo.LpoNubmer(prn.PurchaseReqNoteId + item.PRNDetailsId);
								
								if(await _repo.AddUpdatePrnDetailsAsync(item))
								{
									var lpoObject = _repo.BuildLPODomianObject(item, prn.DeliveryLocation, prn.ExpectedDeliveryDate??DateTime.Today);
									item.PurchaseReqNoteId = prn.PurchaseReqNoteId;
									if (await _repo.AddUpdateLPOAsync(lpoObject))
									{
										var SuggestedSupplierList = lpoObject.SupplierIds.Split(',').Select(int.Parse);
										foreach(var supplierId in SuggestedSupplierList)
										{
											var supplier = await _supRepo.GetSupplierAsync(supplierId);
											if(supplier != null)
											{
												var bidAndTenderObject = _repo.BuildBidAndTenderDomianObject(supplier, lpoObject, prn.DepartmentId, item);
												bidAndTenderObject.CompanyId = prn.CompanyId;
												bidAndTenderObject.PurchaseReqNoteId = prn.PurchaseReqNoteId;
												if (await _repo.AddUpdateBidAndTender(bidAndTenderObject))
												{
													email.ToAddresses.Add(new EmailAddressObj { Address = supplier.Email, Name = supplier.Name }); 
												}
											}
										} 
										var otherSupplierbid = _repo.BuildBidAndTenderDomianObjectForNonSelectedSuppliers(lpoObject, prn.DepartmentId, item); 
										await _repo.AddUpdateBidAndTender(otherSupplierbid);
									}
								} 
							}
							email.ToAddresses.Distinct();
							await _repo.SendEmailToSuppliersAsync(email, prn.Description);
							await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
							await _repo.SendEmailToInitiatingStaffAsync(prn);
							await _trans.CommitAsync();
							return new StaffApprovalRegRespObj
							{
								ResponseId = (int)ApprovalStatus.Approved,
								Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Final Approval </br> Successfully created Supplier Bids" } }
							};
						}
						if (response.ResponseId == (int)ApprovalStatus.Disapproved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							prn.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
							await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
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
						throw ex;
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

		private cor_approvaldetail BuildApprovalDetailObject(PRNStaffApprovalCommand request, purch_requisitionnote currentItem, int staffId)
		{
			var approvalDeatil = new cor_approvaldetail();
			var previousDetail = _detailService.GetApprovalDetailsAsync(request.TargetId, currentItem.WorkflowToken).Result;
			approvalDeatil.ArrivalDate = currentItem.CreatedOn?? DateTime.Now;

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
