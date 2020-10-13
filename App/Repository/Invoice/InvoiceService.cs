using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Contracts.Queries.Finanace;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Repository.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly DataContext _dataContext;
        private readonly ISupplierRepository _suprepo;
        private readonly IPurchaseService _repo;
        private readonly IFinanceServerRequest _financeServer;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly ILoggerService _logger;
        public InvoiceService(
            DataContext dataContext,
            ISupplierRepository repository, 
            IPurchaseService purchaseService,
            IIdentityServerRequest serverRequest,
            ILoggerService loggerService,
            IFinanceServerRequest financeServer)
        {
            _dataContext = dataContext;
            _serverRequest = serverRequest;
            _repo = purchaseService;
            _logger = loggerService;
            _suprepo = repository;
            _financeServer = financeServer;
        }
        public async Task<bool> CreateUpdateInvoiceAsync(inv_invoice model)
        {
            if (model.InvoiceId > 0)
            {
                var itemToUpdate = await _dataContext.inv_invoice.FindAsync(model.InvoiceId);
                _dataContext.Entry(itemToUpdate).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.inv_invoice.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<inv_invoice>> GetAllInvoiceAsync()
        {
            return await _dataContext.inv_invoice.OrderByDescending(a => a.InvoiceId).ToListAsync();
        }
        public async Task<inv_invoice> GetInvoiceAsync(int invoiceId)
        {
            return await _dataContext.inv_invoice.FirstOrDefaultAsync(d => d.InvoiceId == invoiceId);
        } 
        public async Task<IEnumerable<inv_invoice>> GetInvoiceAwaitingApprovalAsync(List<int> invoiceIds, List<string> tokens)
        {
            var item = await _dataContext.inv_invoice
                .Where(s => invoiceIds.Contains(s.InvoiceId)
                && tokens.Contains(s.WorkflowToken)).ToListAsync();
            return item;
        }
        public async Task<inv_invoice> GetInvoiceByPhaseAsync(int paymenttermId)
        {
            return await _dataContext.inv_invoice.FirstOrDefaultAsync(d => d.PaymentTermId == paymenttermId);
        }
        public FinancialTransaction BuildSupplierFirstEntryRequestObject(cor_paymentterms request, inv_invoice invoice)
        {
            cor_suppliertype suppliertype = new cor_suppliertype();
            var lpo = _repo.GetLPOsAsync(request.LPOId).Result;
            var supplierDetail = _suprepo.GetSupplierAsync(lpo.WinnerSupplierId).Result;
            if (supplierDetail != null)
            {
                suppliertype = _suprepo.GetSupplierTypeAsync(supplierDetail.SupplierTypeId).Result;
            } 
            return new FinancialTransaction
            {
                Amount = request.NetAmount,
                ApprovedBy = "staff",
                ApprovedDate = DateTime.Now,
                CompanyId = lpo.CompanyId,
                CreditGL = suppliertype.CreditGL,
                DebitGL =  suppliertype.DebitGL,
                CurrencyId = 0,
                ReferenceNo = invoice.InvoiceNumber,
                Description = $"Proposal Phase {request.Phase}",
                OperationId = (int)OperationsEnum.PaymentApproval,
                TransactionDate = DateTime.Now,

            };
        }
        public List<FinancialTransaction> BuildTaxEntryRequestObject(inv_invoice invoice)
        {
            var supplierType = new cor_suppliertype();
            List<cor_taxsetup> tx = new List<cor_taxsetup>();
            List<FinancialTransaction> fts = new List<FinancialTransaction>();
            var supplierDetail = _suprepo.GetSupplierAsync(invoice.SupplierId).Result;
            if (supplierDetail != null)
            {
                var phase = _dataContext.cor_paymentterms.FirstOrDefault(d => d.PaymentTermId == invoice.PaymentTermId);
                var lpo = _dataContext.purch_plpo.FirstOrDefault(r => r.PLPOId == phase.LPOId);
                supplierType = _dataContext.cor_suppliertype.FirstOrDefault(rt => rt.SupplierTypeId == supplierDetail.SupplierTypeId);
                if (!string.IsNullOrEmpty(lpo.Taxes))
                    {
                    var applicableTax = lpo.Taxes.Split(',').Select(int.Parse).ToList();
                    tx = _dataContext.cor_taxsetup.Where(q => applicableTax.Contains(q.TaxSetupId)).ToList();
                } 
            }
            if (tx.Count() > 0)
            {
                var banks = _financeServer.GetAllBanksAsync().Result;
                var creditGl = banks.bank.FirstOrDefault(q => q.BankGlId == invoice.PaymentBankId).SubGl;
                var currentPhase = _dataContext.cor_paymentterms.FirstOrDefault(q => q.PaymentTermId == invoice.PaymentTermId);
                foreach(var tax in tx)
                {
                    var entryobj = new FinancialTransaction
                    {
                        Amount =  currentPhase.Amount / 100 * Convert.ToDecimal(tax.Percentage),
                        ApprovedBy = "staff",
                        ApprovedDate = DateTime.Now,
                        CompanyId = invoice.CompanyId,
                        CreditGL = tax.SubGL,
                        DebitGL = supplierType.CreditGL,
                        CurrencyId = 0, 
                        ReferenceNo = invoice.InvoiceNumber,
                        Description = $"Invoice {invoice.InvoiceNumber} {tax.TaxName} Tax",
                        OperationId = (int)OperationsEnum.PaymentApproval,
                        TransactionDate = DateTime.Now,
                    };
                    fts.Add(entryobj);
                }
                return fts;
            }
            return new List<FinancialTransaction>();
        }
        public FinancialTransaction BuildSupplierSecondEntryRequestObject(inv_invoice invoice)
        {
            var banks =  _financeServer.GetAllBanksAsync().Result;
            var creditGl = banks.bank.FirstOrDefault(q => q.BankGlId == invoice.PaymentBankId).SubGl;
            var currentPhase = _dataContext.cor_paymentterms.FirstOrDefault(q => q.PaymentTermId == invoice.PaymentTermId);
            return new FinancialTransaction
            {
                Amount = currentPhase.Amount,
                ApprovedBy = "staff",
                ApprovedDate = DateTime.Now,
                CompanyId = invoice.CompanyId,
                CreditGL = creditGl,
                DebitGL = invoice.CreditGl,
                CurrencyId = 0,
                ReferenceNo = invoice.InvoiceNumber,
                Description = $"Invoice {invoice.InvoiceNumber}",
                OperationId = (int)OperationsEnum.PaymentApproval,
                TransactionDate = DateTime.Now,

            };
        }
        public async Task<PaymentTermsRegRespObj> TransferPaymentAsync(inv_invoice invoice)
        {
            var paymentResponse = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            try
            {
                var banks = await _financeServer.GetAllBanksAsync();
                var currencies = await _serverRequest.GetSingleCurrencyAsync(invoice.CurrencyId);
                var CurrencyCode = currencies.commonLookups.FirstOrDefault(q => q.LookupId == invoice.CurrencyId)?.code;
                var suplierBankDetails = _dataContext.cor_bankaccountdetail.Where(d => d.SupplierId == invoice.SupplierId).ToList();
                if (suplierBankDetails.Count() == 0)
                {
                    paymentResponse.Status.Message.FriendlyMessage = "UnIdentified Supplier Bank Details";
                    return paymentResponse;
                }
                var singlesupbnDet = suplierBankDetails.FirstOrDefault(a => invoice.SupplierBankId == a.BankAccountDetailId);
                if (singlesupbnDet == null)
                {
                    paymentResponse.Status.Message.FriendlyMessage = "Unable to find Supplier Bank Details";
                    return paymentResponse;
                }
                if (string.IsNullOrEmpty(CurrencyCode))
                {
                    paymentResponse.Status.Message.FriendlyMessage = "UnIdentified Currency";
                    return paymentResponse;
                }
                if (banks.bank.Count() > 0)
                {
                    if (!banks.bank.Select(w => w.BankGlId).Contains(singlesupbnDet.Bank))
                    {
                        paymentResponse.Status.Message.FriendlyMessage = "UnIdentified Supplier Bank Details";
                        return paymentResponse;
                    }
                    var trans = new TransferObj();

                    trans.account_bank = banks.bank.FirstOrDefault(q => q.BankGlId == invoice.PaymentBankId)?.BankCode;
                    trans.account_number = singlesupbnDet.AccountNumber;
                    trans.amount = Convert.ToInt64(invoice.Amount);
                    trans.currency = CurrencyCode;
                    trans.debit_currency = CurrencyCode;
                    trans.narration = $"'{invoice.Description}' Payment";
                    trans.reference = invoice.InvoiceNumber;
                    var res = await _financeServer.MakePaymentsAsync(trans);
                    if (res.status == null || !res.status.Contains("success"))
                    {
                        paymentResponse.Status.Message.FriendlyMessage = res.message;
                        //return paymentResponse;
                    }

                    var entryRequest1 = BuildSupplierSecondEntryRequestObject(invoice);
                    var invoiceentry = await _financeServer.PassEntryAsync(entryRequest1);
                    if (!invoiceentry.Status.IsSuccessful)
                    {
                        paymentResponse.Status.IsSuccessful = false;
                        paymentResponse.Status.Message.FriendlyMessage = $"Unable to pass invoice entry : '{invoiceentry.Status.Message.FriendlyMessage}'";
                        _logger.Information(invoiceentry.Status.Message.FriendlyMessage);
                        return paymentResponse;
                    }
                    var entryRequest2 = BuildTaxEntryRequestObject(invoice);
                    if(entryRequest2.Count() > 0)
                    {
                        foreach(var tx in entryRequest2)
                        {
                            var taxEntry = await _financeServer.PassEntryAsync(tx);
                            if (!taxEntry.Status.IsSuccessful)
                            {
                                paymentResponse.Status.IsSuccessful = false;
                                paymentResponse.Status.Message.FriendlyMessage = $"Unable to pass tax entry : '{taxEntry.Status.Message.FriendlyMessage}'";
                                _logger.Information(taxEntry.Status.Message.FriendlyMessage);
                                return paymentResponse;
                            }
                        } 
                    }
                    paymentResponse.Status.IsSuccessful = true;
                    return paymentResponse;
                }
                else
                {
                    paymentResponse.Status.Message.FriendlyMessage = "No bank Found";
                    return paymentResponse;
                }
            }
            catch (Exception ex)
            {
                paymentResponse.Status.Message.FriendlyMessage = ex?.Message;
                return paymentResponse;
            }
          
            
        }
    }

    public interface IInvoiceService
    {
        Task<bool> CreateUpdateInvoiceAsync(inv_invoice model);
        Task<IEnumerable<inv_invoice>> GetAllInvoiceAsync();
        Task<inv_invoice> GetInvoiceAsync(int invoiceId);
        Task<inv_invoice> GetInvoiceByPhaseAsync(int paymenttermId);
        Task<IEnumerable<inv_invoice>> GetInvoiceAwaitingApprovalAsync(List<int> invoiceIds, List<string> tokens);
        FinancialTransaction BuildSupplierFirstEntryRequestObject(cor_paymentterms request, inv_invoice invoice);
        FinancialTransaction BuildSupplierSecondEntryRequestObject(inv_invoice invoice);
        Task<PaymentTermsRegRespObj> TransferPaymentAsync(inv_invoice invoice); 
        List<FinancialTransaction> BuildTaxEntryRequestObject(inv_invoice invoice);

    }
}
