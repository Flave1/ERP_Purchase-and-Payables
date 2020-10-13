using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using MediatR;
using Newtonsoft.Json;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase.Payment_proposals
{
    public class GetPaymentProposalAwaitingApprovalQuery : IRequest<PaymentApprovalRespObj>
    {
        public class GetPaymentProposalAwaitingApprovalQueryHandler : IRequestHandler<GetPaymentProposalAwaitingApprovalQuery, PaymentApprovalRespObj>
        {
            private readonly ILoggerService _logger;
            private readonly IInvoiceService _repo;
            private readonly DataContext _dataContext;
            private readonly IIdentityService _identityService; 
            private readonly IIdentityServerRequest _serverRequest;
            private readonly ISupplierRepository _sup;
            private readonly IFinanceServerRequest _financeServer; 

            public GetPaymentProposalAwaitingApprovalQueryHandler(
                ILoggerService loggerService,
                DataContext dataContext,
                IIdentityService identityService,
                ISupplierRepository supplierRepository,
                IInvoiceService invoiceService, 
                IIdentityServerRequest identityServerRequest,
                IFinanceServerRequest financeServer)
            {
                _dataContext = dataContext;
                _logger = loggerService;
                _sup = supplierRepository;
                _repo = invoiceService;
                _financeServer = financeServer;
                _identityService = identityService;
                _serverRequest = identityServerRequest;
            }

            public async Task<PaymentApprovalRespObj> Handle(GetPaymentProposalAwaitingApprovalQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var apiResponse = new PaymentApprovalRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };

                    var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                    if (!result.IsSuccessStatusCode)
                    {
                        apiResponse.Status.Message.FriendlyMessage = $"{result.ReasonPhrase} {result.StatusCode}";
                        return apiResponse;
                    }
                    var data = await result.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data);

                    if (res == null)
                    {
                        apiResponse.Status = res.Status;
                        return apiResponse;
                    }
                    var banks = await _financeServer.GetAllBanksAsync();
                    if (res.workflowTasks.Count() < 1)
                    {
                        apiResponse.Status.IsSuccessful = true;
                        apiResponse.Status.Message.FriendlyMessage = "No Pending Approval";
                        return apiResponse;
                    }  

                    var pendingTaskIds = res.workflowTasks.Select(x => x.TargetId).ToList();
                    var pendingTaskTokens = res.workflowTasks.Select(s => s.WorkflowToken).ToList();
                    var inv = await _repo.GetInvoiceAwaitingApprovalAsync(pendingTaskIds, pendingTaskTokens);
                    if(inv.Count() == 0)
                    {
                        apiResponse.Status.IsSuccessful = true;
                        apiResponse.Status.Message.FriendlyMessage = "No Payments awaiting approvals";
                        return apiResponse;
                    }
                    var phases = _dataContext.cor_paymentterms.Where(a => inv.Select(e => e.PaymentTermId).Contains(a.PaymentTermId) && a.ProposedBy == (int)Proposer.STAFF).ToList();

                    var result1 = (from a in phases
                                   join d in inv on a.PaymentTermId equals d.PaymentTermId 
                                   select new InvoiceObj 
                                   {
                                       AmountPaid = d.AmountPayable,
                                       AmountPayable = d.AmountPayable,
                                       InvoiceId = d.InvoiceId,
                                       DescriptionOfRequest = d.Description,
                                       ExpectedDeliveryDate = d.DeliveryDate,
                                       Location = d.Address,
                                       LPONumber = d.LPONumber,
                                       PaymentOutstanding = 0,
                                       InvoiceNumber = d.InvoiceNumber,
                                       PaymentTermId = d.PaymentTermId,
                                       Amount = d.Amount,
                                       RequestDate = d.RequestDate,
                                       Workflowtoken = d.WorkflowToken,
                                       SupplierId = d.SupplierId,
                                       LpoId = a.LPOId,
                                       PaymentBankId = d.SupplierBankId
                                   }).ToList();

                    
                    if (result1.Count() > 0)
                    {
                        foreach (var item in result1)
                        {
                            var alreadyPaidPhases = _dataContext.cor_paymentterms.Where(a => a.PaymentTermId == item.PaymentTermId && a.ProposedBy == (int)Proposer.STAFF && a.PaymentStatus == (int)PaymentStatus.Paid).ToList();
                            item.BankName = banks.bank.FirstOrDefault(q => q.BankGlId == item.SupplierBankId)?.BankName;
                            item.Supplier = _dataContext.cor_supplier.FirstOrDefault(q => q.SupplierId == item.SupplierId)?.Name;
                            item.AmountPaid = alreadyPaidPhases.Sum(f => f.NetAmount);
                            item.PaymentOutstanding = (item.AmountPayable - alreadyPaidPhases.Sum(f => f.NetAmount)); 
                        }
                        apiResponse.PaymentApprovals = result1;
                    }
                    

                    //resp.Add(item);

                    apiResponse.Status.IsSuccessful = true;
                    apiResponse.Status.Message.FriendlyMessage = inv.Count() < 1 ? "No Payments awaiting approvals" : null;
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
 
