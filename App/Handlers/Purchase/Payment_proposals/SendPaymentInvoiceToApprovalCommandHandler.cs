using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; 
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Payment; 
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
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
      public class SendPaymentInvoiceToApprovalCommand : IRequest<PaymentTermsRegRespObj> 
      {
        public int PaymentTermId { get; set; }
        public int PaymentBankId { get; set; }
        public int SupplierBankId { get; set; } 
        public class SendPaymentInvoiceToApprovalCommandHandler : IRequestHandler<SendPaymentInvoiceToApprovalCommand, PaymentTermsRegRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly ILoggerService _logger;
            private readonly DataContext _dataContext;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IInvoiceService _invoice;
            private readonly IFinanceServerRequest _financeServer;
            public SendPaymentInvoiceToApprovalCommandHandler(
                IPurchaseService purchaseService,
                ILoggerService loggerService,
                DataContext dataContext,
                IIdentityServerRequest serverRequest,
                IInvoiceService  invoice,
                IFinanceServerRequest financeServer)
            {
                _invoice = invoice;
                _repo = purchaseService;
                _dataContext = dataContext;
                _logger = loggerService;
                _financeServer = financeServer;
                _serverRequest = serverRequest;
            }

              
            public async Task<PaymentTermsRegRespObj> Handle(SendPaymentInvoiceToApprovalCommand request, CancellationToken cancellationToken)
            {
                var apiResponse = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var paymentprop = await _repo.GetSinglePaymenttermAsync(request.PaymentTermId);

                    if (paymentprop == null)
                    {
                        apiResponse.Status.Message.FriendlyMessage = $"Proposal Not found";
                        return apiResponse;
                    }
                    if (paymentprop.PaymentStatus == (int)PaymentStatus.Paid)
                    {
                        apiResponse.Status.Message.FriendlyMessage = "Payment already made for this phase";
                        return apiResponse;
                    }
                    if (paymentprop.ApprovalStatusId == (int)ApprovalStatus.Processing)
                    {
                        apiResponse.Status.Message.FriendlyMessage = "Payment In Process";
                        return apiResponse;
                    }
                    var invoice = await _invoice.GetInvoiceByPhaseAsync(paymentprop.PaymentTermId);

                    if (invoice == null)
                    {
                        apiResponse.Status.Message.FriendlyMessage = "Invoice not found";
                        return apiResponse;
                    }

                    if (invoice.ApprovalStatusId != (int)ApprovalStatus.Approved)
                    {
                        if (invoice.ApprovalStatusId != (int)ApprovalStatus.Pending)
                        {
                            apiResponse.Status.Message.FriendlyMessage = $"Unable to push invoice with status '{Convert.ToString((ApprovalStatus)invoice.ApprovalStatusId)}' for approval";
                            return apiResponse;
                        }
                    }
                    var companies = await _serverRequest.GetAllCompanyStructureAsync();
                    var currentCompanyCurrency = companies.companyStructures.FirstOrDefault(q => q.CompanyStructureId == invoice.CompanyId);
                    if (currentCompanyCurrency == null)
                    {
                        apiResponse.Status.Message.FriendlyMessage = "Unable to Identify Company";
                        return apiResponse;
                    }
                    invoice.CurrencyId = currentCompanyCurrency.ReportCurrencyId ?? 0;

                    invoice.PaymentBankId = request.PaymentBankId;
                    invoice.SupplierBankId = request.SupplierBankId; 
                    var user = await _serverRequest.UserDataAsync();

                    using (var _transaction = await _dataContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var targetList = new List<int>();
                            targetList.Add(invoice.InvoiceId);
                            GoForApprovalRequest wfRequest = new GoForApprovalRequest
                            {
                                Comment = "Invoice Payment Phase",
                                OperationId = (int)OperationsEnum.PaymentApproval,
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
                            GoForApprovalRespObj res = JsonConvert.DeserializeObject<GoForApprovalRespObj>(stringData);

                            if (res.ApprovalProcessStarted)
                            {
                                invoice.ApprovalStatusId = (int)ApprovalStatus.Processing;
                                invoice.WorkflowToken = res.Status.CustomToken;
                                paymentprop.ApprovalStatusId = (int)ApprovalStatus.Processing;
                                await _repo.AddUpdatePaymentTermsAsync(paymentprop); 
                                await _invoice.CreateUpdateInvoiceAsync(invoice);
                                await _transaction.CommitAsync();

                                apiResponse.PaymentTermId = invoice.PaymentTermId;
                                apiResponse.Status = res.Status;
                                return apiResponse;
                            }

                            if (res.EnableWorkflow || !res.HasWorkflowAccess)
                            {
                                await _transaction.RollbackAsync();
                                apiResponse.Status.Message = res.Status.Message;
                                return apiResponse;
                            }

                            if (!res.EnableWorkflow)
                            { 
                                invoice.ApprovalStatusId = (int)ApprovalStatus.Approved;

                                var thisInvoicePhase = await _repo.GetSinglePaymenttermAsync(invoice.PaymentTermId);
                                thisInvoicePhase.PaymentStatus = (int)PaymentStatus.Paid;
                                thisInvoicePhase.CompletionDate = DateTime.Now;
                                thisInvoicePhase.ApprovalStatusId = (int)ApprovalStatus.Approved; 
                                
                                var paymentResp = await _invoice.TransferPaymentAsync(invoice);
                                if (!paymentResp.Status.IsSuccessful)
                                {
                                    await _transaction.RollbackAsync();
                                    apiResponse.Status.IsSuccessful = false;
                                    apiResponse.Status.Message.FriendlyMessage = paymentResp.Status.Message.FriendlyMessage;
                                    return apiResponse;
                                } 
                                await _repo.AddUpdatePaymentTermsAsync(thisInvoicePhase);
                                invoice.AmountPaid = invoice.Amount;
                                await _invoice.CreateUpdateInvoiceAsync(invoice);

                                var entryRequest = _invoice.BuildSupplierSecondEntryRequestObject(invoice);
                                var entry = await _financeServer.PassEntryAsync(entryRequest);
                                if (!entry.Status.IsSuccessful)
                                {
                                    await _transaction.RollbackAsync();
                                    apiResponse.Status.IsSuccessful = false;
                                    apiResponse.Status.Message.FriendlyMessage = $"{entry.Status.Message.FriendlyMessage}";
                                    return apiResponse;
                                }
                                await _repo.SendEmailToSupplierDetailingPaymentAsync(invoice, thisInvoicePhase.Phase);
                                await _transaction.CommitAsync();
                                apiResponse.Status.IsSuccessful = true;
                                apiResponse.Status.Message.FriendlyMessage = $"Payment Successful";
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
                            return new PaymentTermsRegRespObj
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
                    return new PaymentTermsRegRespObj
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
    
}
