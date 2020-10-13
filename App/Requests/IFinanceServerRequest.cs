using Puchase_and_payables.Contracts.Queries.Finanace;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Puchase_and_payables.Contracts.Response.FinanceServer.Banks;
using static Puchase_and_payables.Handlers.Report.PayableDaysCalcultion.PayableDaysCalcultionHandler;

namespace Puchase_and_payables.Requests
{
    public interface IFinanceServerRequest
    {
        Task<InvoiceRegRespObj> CreateUpdateInvoiceAsync(InvoiceRegObj request);
        Task<SubGlRespObj> GetAllSubglAsync();
        Task<FinancialTransactionRegObj> PassEntryAsync(FinancialTransaction request);  
        Task<PaidRespObj> MakePaymentsAsync(TransferObj request);
        Task<BanksRespObj> GetAllBanksAsync();
        Task<FinancialEntriesResp> GetFinTransactions(ReportGls gls);
        Task<AccountNumberVerification> VerifyAccountNumber(VerifyAccount request);

    }
}
