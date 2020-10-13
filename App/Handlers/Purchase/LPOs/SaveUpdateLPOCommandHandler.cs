using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR; 
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Payment; 
using Puchase_and_payables.Data; 
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class SaveUpdateLPOCommand : IRequest<PaymentTermsRegRespObj>
    {
        
        public List<int> TaxId { get; set; } 
        public int LPOId { get; set; }
        public int ServicetermsId { get; set; }
        public class SaveUpdateLPOCommandHandler : IRequestHandler<SaveUpdateLPOCommand, PaymentTermsRegRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly ISupplierRepository _suprepo;
            private readonly ILoggerService _logger;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IFinanceServerRequest _financeServer;
            private readonly DataContext _dataContext;
            public SaveUpdateLPOCommandHandler(
                IPurchaseService purchaseService,
                ILoggerService loggerService, 
                IIdentityServerRequest serverRequest,
                IFinanceServerRequest financeServer,
                DataContext dataContext,
                ISupplierRepository supplierRepository
                )
            {
                _suprepo = supplierRepository;
                _dataContext = dataContext;
                _logger = loggerService;
                _financeServer = financeServer;
                _repo = purchaseService;
                _serverRequest = serverRequest;
            }
            private async Task<List<cor_taxsetup>> GetAppliedTaxes(List<int> taxIds)
            { 
                if (taxIds.Count() > 0)
                {
                    var ListOfTaxapplicable = new List<cor_taxsetup>();
                    foreach (var tId in taxIds)
                    {
                        var taxApplicable = await _dataContext.cor_taxsetup.FirstOrDefaultAsync(q => q.TaxSetupId == tId);  
                        if(taxApplicable != null)
                        {
                            ListOfTaxapplicable.Add(taxApplicable);
                        }
                    }
                    return ListOfTaxapplicable;
                }
                return new List<cor_taxsetup>();
            }

            public async Task<PaymentTermsRegRespObj> Handle(SaveUpdateLPOCommand request, CancellationToken cancellationToken)
            {
                var response = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var user = await _serverRequest.UserDataAsync();
                    if (user == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Process This User";
                        return response; 
                    } 
                    purch_plpo currentlpo = new purch_plpo();
                    currentlpo = await _repo.GetLPOsAsync(request.LPOId);
                    if (currentlpo == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Identify LPO associated to this payment proposal (s)";
                        return response;
                    }
                    var phases = _dataContext.cor_paymentterms.Where(q => q.LPOId == currentlpo.PLPOId && q.ProposedBy == (int)Proposer.STAFF).ToList();
                    if (phases.Count() > 0 && phases.Any(q => q.PaymentStatus == (int)PaymentStatus.Not_Paid || q.PaymentStatus == (int)PaymentStatus.Paid))
                    {
                        response.Status.Message.FriendlyMessage = "Payment process already started for this LPO";
                        return response;
                    }

                    if (!string.IsNullOrEmpty(currentlpo.Taxes))
                    {
                        if(request.TaxId.Count() > 0)
                        {
                            var taxselected = currentlpo.Taxes.Split(',').Select(int.Parse).Except(request.TaxId).ToList();
                            currentlpo.Taxes = string.Join(',', taxselected);
                            if (taxselected.Count() > 0)
                            { 
                                var appliedTaxes = await GetAppliedTaxes(taxselected); 
                                var lpo = ReturnAmountPayable(appliedTaxes, currentlpo);
                                currentlpo.AmountPayable = lpo.AmountPayable;
                                currentlpo.Tax = lpo.Tax;
                            }
                            if (taxselected.Count() == 0)
                            {
                                currentlpo.AmountPayable = currentlpo.GrossAmount;
                                currentlpo.Tax = 0;
                            } 
                        } 
                    }
                    currentlpo.ServiceTerm = request.ServicetermsId;
                    using (var _transaction = await _dataContext.Database.BeginTransactionAsync())
                    {
                        try
                        { 
                            var targetList = new List<int>();
                            targetList.Add(currentlpo.PLPOId);
                            GoForApprovalRequest wfRequest = new GoForApprovalRequest
                            {
                                Comment = "Bidded LPO",
                                OperationId = (int)OperationsEnum.PurchaseLPOApproval,
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
                                response.Status.IsSuccessful = false;
                                response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}";
                                return response; 
                            }
                            var stringData = await result.Content.ReadAsStringAsync();
                            GoForApprovalRespObj res = JsonConvert.DeserializeObject<GoForApprovalRespObj>(stringData);

                            if (res.ApprovalProcessStarted)
                            {
                                currentlpo.ApprovalStatusId = (int)ApprovalStatus.Processing;
                                currentlpo.WorkflowToken = res.Status.CustomToken;
                                await _repo.AddUpdateLPOAsync(currentlpo);
                                await _transaction.CommitAsync();
                                 
                                response.Status.IsSuccessful = res.Status.IsSuccessful;
                                response.Status.Message = res.Status.Message;
                                return response; 
                            } 

                            if (res.EnableWorkflow || !res.HasWorkflowAccess)
                            {
                                response.Status.IsSuccessful = res.Status.IsSuccessful;
                                response.Status.Message = res.Status.Message;
                                return response;
                            }
                            if (!res.EnableWorkflow)
                            {
                                currentlpo.ApprovalStatusId = (int)ApprovalStatus.Approved;
                                await _repo.AddUpdateLPOAsync(currentlpo);
                                await _repo.ShareTaxToPhasesIthereIsAsync(currentlpo);
                                await _repo.RemoveLostBidsAndProposals(currentlpo);
                                await _transaction.CommitAsync();
                                response.Status.IsSuccessful = true;
                                response.Status.Message.FriendlyMessage = "LPO Updated";
                                return response;
                            }
                            response.Status.IsSuccessful = res.Status.IsSuccessful;
                            response.Status.Message = res.Status.Message;
                            return response; 
                        }
                        catch (Exception ex)
                        {
                            await _transaction.RollbackAsync();
                            #region Log error to file 
                            var errorCode = ErrorID.Generate(4);
                            _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                            response.Status.Message.FriendlyMessage = "Error occured!! Please try again later";
                            response.Status.Message.MessageId = errorCode;
                            response.Status.Message.TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                            return response;
                         
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
            private purch_plpo ReturnAmountPayable(List<cor_taxsetup> TaxSetup, purch_plpo thisBidLpo)
            {
                if (TaxSetup.Count() > 0)
                {
                    decimal charge = new decimal();
                    decimal deduction = new decimal();
                    thisBidLpo.Taxes = string.Join(',', TaxSetup.Select(s => s.TaxSetupId));
                    foreach (var tax in TaxSetup)
                    {
                        if (tax.Type == "+")
                        {
                            charge = thisBidLpo.GrossAmount / 100 * Convert.ToDecimal(tax.Percentage);
                        }
                        if (tax.Type == "-")
                        {
                            deduction = thisBidLpo.GrossAmount / 100 * Convert.ToDecimal(tax.Percentage);
                        }
                    }
                    thisBidLpo.AmountPayable = thisBidLpo.GrossAmount - deduction + charge;
                    thisBidLpo.Tax = (thisBidLpo.AmountPayable - thisBidLpo.GrossAmount);
                    return thisBidLpo;
                }
                return new purch_plpo();
            }

        }

        
    }
    
}