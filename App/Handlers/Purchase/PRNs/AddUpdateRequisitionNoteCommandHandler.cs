using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service; 
using GOSLibraries.URI;
using MediatR; 
using Microsoft.Data.SqlClient;
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Purchase; 
using Puchase_and_payables.Helper.Extensions;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class AddUpdateRequisitionNoteCommandHandler : IRequestHandler<AddUpdateRequisitionNoteCommand, RequisitionNoteRegRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly ILoggerService _logger;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly DataContext _dataContext;
        private readonly ISupplierRepository _suprepo; 
        public AddUpdateRequisitionNoteCommandHandler(
            IPurchaseService purchaseService,
            ISupplierRepository repository,
            ILoggerService loggerService, 
            DataContext dataContext,
            IIdentityServerRequest serverRequest)
        {
            _logger = loggerService;
            _repo = purchaseService;
            _suprepo = repository;
            _serverRequest = serverRequest;
            _dataContext = dataContext; 
        }

      
        public async Task<RequisitionNoteRegRespObj> Handle(AddUpdateRequisitionNoteCommand request, CancellationToken cancellationToken)
        {
			try
			{
                var apiResponse = new RequisitionNoteRegRespObj { Status = new APIResponseStatus { IsSuccessful  = false, Message = new APIResponseMessage() } };
                var user = await _serverRequest.UserDataAsync();
                if (user == null)
                {
                    return new RequisitionNoteRegRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = " Unable To Process This User" } }
                    };
                }

                List<purch_prndetails> prnDetails = new List<purch_prndetails>(); 
               using (var _trans = await _dataContext.Database.BeginTransactionAsync())
                {
                    try
                    { 
                        var prn = _repo.BuildPurchRequisionNoteObject(request, user.StaffId, user.CompanyId);
                        await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);

                        if (request.PRNDetails.Count() > 0)
                        {
                             prnDetails = _repo.BuildListOfPrnDetails(request.PRNDetails, prn.CompanyId);  
                        } 
                        var targetList = new List<int>();
                        targetList.Add(prn.PurchaseReqNoteId);
                        GoForApprovalRequest wfRequest = new GoForApprovalRequest
                        {
                            Comment = "Purchase PRN",
                            OperationId = (int)OperationsEnum.PurchasePRNApproval,
                            TargetId = targetList,
                            ApprovalStatus = (int)ApprovalStatus.Pending,
                            DeferredExecution = true,
                            StaffId = user.StaffId,
                            CompanyId = user.CompanyId,
                            EmailNotification = true,
                            ExternalInitialization = false,
                            StatusId = (int)ApprovalStatus.Processing,
                        };
                        var result = await _serverRequest.GotForApprovalAsync(wfRequest);
                        if (!result.IsSuccessStatusCode)
                        {
                            apiResponse.Status.Message.FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}";
                            return apiResponse;
                        }
                        var stringData = await result.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<GoForApprovalRespObj>(stringData);
                        if (res.ApprovalProcessStarted)
                        {
                            prn.ApprovalStatusId = (int)ApprovalStatus.Processing;
                            prn.WorkflowToken = res.Status.CustomToken;

                            await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
                            foreach (var item in prnDetails)
                            {
                                item.PurchaseReqNoteId = prn.PurchaseReqNoteId;
                                await _repo.AddUpdatePrnDetailsAsync(item);
                            } 
                            await _trans.CommitAsync();

                            apiResponse.PurchaseReqNoteId = prn.PurchaseReqNoteId;
                            apiResponse.Status.IsSuccessful = res.Status.IsSuccessful;
                            apiResponse.Status.Message = res.Status.Message; 
                            return apiResponse;
                        }

                        if (res.EnableWorkflow || !res.HasWorkflowAccess)
                        {
                            await _trans.CommitAsync();
                            apiResponse.PurchaseReqNoteId = prn.PurchaseReqNoteId;
                            apiResponse.Status.IsSuccessful = true;
                            apiResponse.Status.Message.FriendlyMessage = "Successful";
                            return apiResponse;
                        }
                        if (!res.EnableWorkflow)
                        { 
                            prn.ApprovalStatusId = (int)ApprovalStatus.Approved; 
                            EmailMessageObj email = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>()};
                            var lponum = 0;
                            foreach (var item in prnDetails)
                            { 
                                if (await _repo.AddUpdatePrnDetailsAsync(item))
                                {
                                    item.LPONumber = _repo.LpoNubmer(prn.PurchaseReqNoteId + item.PRNDetailsId);
                                    item.PurchaseReqNoteId = prn.PurchaseReqNoteId;
                                    await _repo.AddUpdatePrnDetailsAsync(item);
                                    
                                    var lpoObject = _repo.BuildLPODomianObject(item, prn.DeliveryLocation, prn.ExpectedDeliveryDate ?? DateTime.Today);
                                    if (await _repo.AddUpdateLPOAsync(lpoObject))
                                    {
                                        var SuggestedSupplierList = lpoObject.SupplierIds.Split(',').Select(int.Parse);
                                        foreach (var supplierId in SuggestedSupplierList)
                                        {
                                            var supplier = await _suprepo.GetSupplierAsync(supplierId);
                                            if (supplier != null)
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
                                        otherSupplierbid.PurchaseReqNoteId = prn.PurchaseReqNoteId;
                                        otherSupplierbid.CompanyId = prn.CompanyId;
                                        await _repo.AddUpdateBidAndTender(otherSupplierbid);
                                        
                                    }
                                }
                            }
                            email.ToAddresses.Distinct();
                            await _repo.SendEmailToSuppliersAsync(email, prn.Description);

                            await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
                            await _trans.CommitAsync();
                            apiResponse.Status.IsSuccessful = true;
                            apiResponse.Status.Message.FriendlyMessage = "Successful";
                            return apiResponse;
                        }

                    }
                    catch (SqlException ex)
                    {
                        await _trans.RollbackAsync();
                        var errorCode = ErrorID.Generate(4);
                        _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                        throw ex;
                    }
                    finally { await _trans.DisposeAsync(); }
                }
                    return new RequisitionNoteRegRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Success"
                            }
                        }
                    };

            }
            catch (Exception ex)
			{
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new RequisitionNoteRegRespObj
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Unable to process item",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        } 
    }
}
