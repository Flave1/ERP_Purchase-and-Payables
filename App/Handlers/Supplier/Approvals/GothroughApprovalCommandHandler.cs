using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Data;
using Puchase_and_payables.Helper.Extensions;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Approvals
{

    public class GothroughApprovalCommandHandler : IRequestHandler<GothroughApprovalCommand, SupplierRegRespObj>
    {
        private readonly ISupplierRepository _repo;
        private readonly ILoggerService _logger; 
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        public GothroughApprovalCommandHandler(
            ISupplierRepository supplier, 
            ILoggerService loggerService,
            DataContext dataContext,
            IIdentityServerRequest serverRequest)
        {
            _repo = supplier;
            _dataContext = dataContext;
            _logger = loggerService;
            _serverRequest = serverRequest;
        }
        public async Task<SupplierRegRespObj> Handle(GothroughApprovalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var supplierInfor = await _repo.GetSupplierAsync(request.SupplierId);
                if(supplierInfor == null)
                {
                    return new SupplierRegRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = $"Supplier Not found"
                            }
                        }
                    };
                }
                var enumName = (ApprovalStatus)supplierInfor.ApprovalStatusId;
                if (supplierInfor.ApprovalStatusId != (int)ApprovalStatus.Pending)
                {
                    return new SupplierRegRespObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = $"Unable to push supplier with status '{enumName.ToString()}' for approvals"
                            }
                        }
                    };
                }
                var user = await _serverRequest.UserDataAsync();

                using (var _transaction = await _dataContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var targetList = new List<int>();
                        targetList.Add(supplierInfor.SupplierId);
                        GoForApprovalRequest wfRequest = new GoForApprovalRequest
                        {
                            Comment = "Supplier Registration",
                            OperationId = (int)OperationsEnum.SupplierRegistrationApproval,
                            TargetId = targetList,
                            ApprovalStatus = (int)ApprovalStatus.Processing,
                            DeferredExecution = true,
                            StaffId = user.StaffId,
                            CompanyId = user.CompanyId,
                            EmailNotification = false,
                            ExternalInitialization = false,
                            StatusId = (int)ApprovalStatus.Processing,
                        };

                        var result = await _serverRequest.GotForApprovalAsync(wfRequest);

                        if (!result.IsSuccessStatusCode)
                        {
                            new SupplierRegRespObj
                            {
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = false,
                                    Message = new APIResponseMessage { FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}" }
                                }
                            };
                        }
                        var stringData = await result.Content.ReadAsStringAsync();
                        GoForApprovalRespObj res = JsonConvert.DeserializeObject<GoForApprovalRespObj>(stringData);

                        if (res.ApprovalProcessStarted)
                        { 
                            supplierInfor.ApprovalStatusId = (int)ApprovalStatus.Processing;
                            supplierInfor.WorkflowToken = res.Status.CustomToken;
                            await _repo.UpdateSupplierAsync(supplierInfor);
                            await _transaction.CommitAsync();
                            return new SupplierRegRespObj
                            {
                                SupplierId = supplierInfor.SupplierId,
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = res.Status.IsSuccessful,
                                    Message = res.Status.Message
                                }
                            };
                        }

                        if (res.EnableWorkflow || !res.HasWorkflowAccess)
                        {
                            supplierInfor.ApprovalStatusId = (int)ApprovalStatus.Processing;
                            await _repo.UpdateSupplierAsync(supplierInfor);
                            await _transaction.RollbackAsync();
                            return new SupplierRegRespObj
                            {
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = res.Status.IsSuccessful,
                                    Message = res.Status.Message
                                }
                            };
                        }
                        if (!res.EnableWorkflow)
                        {
                            supplierInfor.ApprovalStatusId = (int)ApprovalStatus.Approved;
                            supplierInfor.SupplierNumber = SupplierNumber.Generate(15);
                            await _repo.UpdateSupplierAsync(supplierInfor);
                            await _transaction.CommitAsync();
                            return new SupplierRegRespObj
                            {
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = true,
                                    Message = new APIResponseMessage { FriendlyMessage = "Successful" }
                                }
                            };
                        }
                        return new SupplierRegRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                IsSuccessful = res.Status.IsSuccessful,
                                Message = res.Status.Message
                            }
                        };

                    }
                    catch (Exception ex)
                    {
                        await _transaction.RollbackAsync();
                        #region Log error to file 
                        var errorCode = ErrorID.Generate(4);
                        _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                        return new SupplierRegRespObj
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
                return new SupplierRegRespObj
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
