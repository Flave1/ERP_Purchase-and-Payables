using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data; 
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
  
    public class SendPRNToApprovalCommandHandler : IRequestHandler<SendPRNToApprovalCommand, RequisitionNoteRegRespObj>
    {
        private readonly IPurchaseService _repo;
        private readonly ILoggerService _logger;
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest; 
        public SendPRNToApprovalCommandHandler(
            IPurchaseService purchaseService,
            ILoggerService loggerService,
            DataContext dataContext,
            IIdentityServerRequest serverRequest)
        {
            _repo = purchaseService;
            _dataContext = dataContext;
            _logger = loggerService;
            _serverRequest = serverRequest; 
        }
        public async Task<RequisitionNoteRegRespObj> Handle(SendPRNToApprovalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = new RequisitionNoteRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                var prn = await _repo.GetPurchaseRequisitionNoteAsync(request.PurchaseReqNoteId); 

                if (prn == null)
                {
                    apiResponse.Status.Message.FriendlyMessage = $"Bid Not found";
                    return apiResponse;
                }

                if (prn.ApprovalStatusId != (int)ApprovalStatus.Pending)
                {
                    apiResponse.Status.Message.FriendlyMessage = $"Unable to push supplier bid with status '{Convert.ToString((ApprovalStatus)prn.ApprovalStatusId)}' for approval";
                    return apiResponse;
                }
                var user = await _serverRequest.UserDataAsync();

                using (var _transaction = await _dataContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var targetList = new List<int>();
                        targetList.Add(prn.PurchaseReqNoteId);
                        GoForApprovalRequest wfRequest = new GoForApprovalRequest
                        {
                            Comment = "Bid and tender",
                            OperationId = (int)OperationsEnum.PurchasePRNApproval,
                            TargetId = targetList,
                            ApprovalStatus = (int)ApprovalStatus.Pending,
                            DeferredExecution = true,
                            StaffId = user.StaffId,
                            CompanyId = 1,
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
                        GoForApprovalRespObj res = JsonConvert.DeserializeObject<GoForApprovalRespObj>(stringData);

                        if (res.ApprovalProcessStarted)
                        {
                            prn.ApprovalStatusId = (int)ApprovalStatus.Processing;
                            prn.WorkflowToken = res.Status.CustomToken;  
                            await _repo.AddUpdatePurchaseRequisitionNoteAsync(prn);
                            await _transaction.CommitAsync();
                            apiResponse.PurchaseReqNoteId = prn.PurchaseReqNoteId;
                            apiResponse.Status = res.Status;
                            return apiResponse;
                        } 

                        if (res.EnableWorkflow || !res.HasWorkflowAccess)
                        {
                            await _transaction.RollbackAsync();
                            apiResponse.Status.IsSuccessful = false;
                            apiResponse.Status.Message = res.Status.Message;
                            return apiResponse;
                        }
                        apiResponse.Status = res.Status;
                        return apiResponse;
                    }
                    catch (Exception ex)
                    {
                        await _transaction.RollbackAsync();
                        #region Log error to file 
                        var errorCode = ErrorID.Generate(4);
                        _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                        return new RequisitionNoteRegRespObj
                        {

                            Status = new APIResponseStatus
                            {
                                Message = new APIResponseMessage
                                {
                                    FriendlyMessage = "Error occured!! Please try again later",
                                    MessageId = errorCode,
                                    TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                                }
                            }
                        };
                        #endregion
                    }
                    finally { await _transaction.DisposeAsync(); }
                }
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
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
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
