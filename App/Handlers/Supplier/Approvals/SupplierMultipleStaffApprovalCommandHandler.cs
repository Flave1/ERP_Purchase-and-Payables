using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http; 
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.DomainObjects.Approvals; 
using Puchase_and_payables.Helper.Extensions;
using Puchase_and_payables.Repository.Details;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Approvals
{
	public class SupplierMultipleStaffApprovalCommand : IRequest<StaffApprovalRegRespObj>
	{
		public List<int> TargetIds { get; set; } 

		public class SupplierMultipleStaffApprovalCommandHandler : IRequestHandler<SupplierMultipleStaffApprovalCommand, StaffApprovalRegRespObj>
		{ 
			private readonly ISupplierRepository _repo;
			private readonly IHttpContextAccessor _accessor;
			private readonly IIdentityServerRequest _serverRequest;
			private readonly IWorkflowDetailService _detailService; 
			private StaffApprovalRegRespObj response = new StaffApprovalRegRespObj();
			public SupplierMultipleStaffApprovalCommandHandler(
				IHttpContextAccessor httpContextAccessor, 
				ISupplierRepository supplierRepository,
				IIdentityServerRequest serverRequest,
				IWorkflowDetailService detailService )
			{
				_repo = supplierRepository; 
				_accessor = httpContextAccessor;
				_serverRequest = serverRequest; 
				_detailService = detailService;
				_serverRequest = serverRequest;
			}

			public async Task<StaffApprovalRegRespObj> Handle(SupplierMultipleStaffApprovalCommand request, CancellationToken cancellationToken)
			{
				try
				{
					var apiResponse = new StaffApprovalRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };

					var currentUserId = _accessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
					var user = await _serverRequest.UserDataAsync();
					foreach (var targetId in request.TargetIds)
					{
						var supplier = await _repo.GetSupplierAsync(targetId);
						var detail = BuildApprovalDetailObject(request, supplier, user.StaffId);

						var req = new IndentityServerApprovalCommand
						{
							ApprovalComment = "Ok",
							ApprovalStatus = (int)ApprovalStatus.Approved,
							TargetId = targetId,
							WorkflowToken = supplier.WorkflowToken, 
						};

						var result = await _serverRequest.StaffApprovalRequestAsync(req);

						if (!result.IsSuccessStatusCode)
						{
							apiResponse.Status.Message.FriendlyMessage = result.ReasonPhrase;
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
							supplier.ApprovalStatusId = (int)ApprovalStatus.Processing;
							await _repo.UpdateSupplierAsync(supplier);

							apiResponse.ResponseId = (int)ApprovalStatus.Processing;
							apiResponse.Status.IsSuccessful = true;
							apiResponse.Status.Message = response.Status.Message;
							return apiResponse;
						}

						if (response.ResponseId == (int)ApprovalStatus.Revert)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							supplier.ApprovalStatusId = (int)ApprovalStatus.Revert;
							await _repo.UpdateSupplierAsync(supplier);

							apiResponse.ResponseId = (int)ApprovalStatus.Revert;
							apiResponse.Status.IsSuccessful = true;
							apiResponse.Status.Message = response.Status.Message;
							return apiResponse;
						}

						if (response.ResponseId == (int)ApprovalStatus.Approved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							supplier.ApprovalStatusId = (int)ApprovalStatus.Approved;
							supplier.SupplierNumber = SupplierNumber.Generate(10);
							await _repo.UpdateSupplierAsync(supplier);
 
							 
							//apiResponse.ResponseId = (int)ApprovalStatus.Approved;
							//apiResponse.Status.IsSuccessful = true;
							//apiResponse.Status.Message = response.Status.Message;
							//return apiResponse;
						}

						if (response.ResponseId == (int)ApprovalStatus.Disapproved)
						{
							await _detailService.AddUpdateApprovalDetailsAsync(detail);
							supplier.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
							await _repo.UpdateSupplierAsync(supplier);

							apiResponse.ResponseId = (int)ApprovalStatus.Disapproved;
							apiResponse.Status.IsSuccessful = true;
							apiResponse.Status.Message = response.Status.Message;
							return apiResponse;
						}
					}
					apiResponse.ResponseId = (int)ApprovalStatus.Approved;
					apiResponse.Status = response.Status;
					return apiResponse;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			private cor_approvaldetail BuildApprovalDetailObject(SupplierMultipleStaffApprovalCommand request, cor_supplier currentItem, int staffId)
			{
				var approvalDeatil = new cor_approvaldetail();
				var previousDetail = _detailService.GetApprovalDetailsAsync(currentItem.SupplierId, currentItem.WorkflowToken).Result;
				approvalDeatil.ArrivalDate = currentItem.CreatedOn ?? DateTime.Now;

				if (previousDetail.Count() > 0)
					approvalDeatil.ArrivalDate = previousDetail.OrderByDescending(s => s.ApprovalDetailId).FirstOrDefault().Date;

				approvalDeatil.Comment = "Ok";
				approvalDeatil.Date = DateTime.Today;
				approvalDeatil.StatusId = (int)ApprovalStatus.Approved;
				approvalDeatil.TargetId = currentItem.SupplierId;
				approvalDeatil.StaffId = staffId;
				approvalDeatil.WorkflowToken = currentItem.WorkflowToken; 
				return approvalDeatil;
			}
			private async Task SendMail(string email, string name)
			{
				var sm = new EmailMessageObj();
				sm.Subject = $" Supplier account details";
				sm.Content = $"Hello {name} <br/>" +
					$"Your supplier account has sussefully being approved. <br/>" +
					$"Below is your account login details" +
					 $"<b>Username : {email} <br/>" +
					$"<b>Password : Password@1 <br/>" +
					$"Please be sure to change your password on first login"; ;
				sm.ToAddresses.Add(new EmailAddressObj { Address = email, Name = name });
				await _serverRequest.SendMessageAsync(sm);
			}
		}
	}
	
}
