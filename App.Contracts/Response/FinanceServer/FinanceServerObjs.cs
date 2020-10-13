using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.FinanceServer
{
    public class SubGl
    {
        public int subGLId { get; set; }
        public string subGLCode { get; set; }
        public string subGLName { get; set; }
        public int glId { get; set; }
        public string glName { get; set; }
        public object noteLine { get; set; }
        public object fsLineCaption { get; set; }
        public int companyId { get; set; }
        public int companyStructureId { get; set; }
        public int glClassId { get; set; }
        public object glClassName { get; set; }
        public string companyName { get; set; }
        public string subGLFullName { get; set; }
        public object active { get; set; }
        public object deleted { get; set; }
        public object createdBy { get; set; }
        public object createdOn { get; set; }
        public object updatedBy { get; set; }
        public object updatedOn { get; set; }
    }

    

    public class SubGlRespObj
    {
        public List<SubGl> SubGls { get; set; }
        public APIResponseStatus Status { get; set; } 
    }
    public class FinancialTransactionRegObj 
    {
        public int TransactionId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class FinancialTransaction
    {
        public string ReferenceNo { get; set; }
        public bool IsCustomerTransaction { get; set; }
        public int SubGLId { get; set; }

        public int OperationId { get; set; }

        public int? CasaAccountId { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public decimal? RunningBalance { get; set; }

        public string Description { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime PostedDate { get; set; }

        public int CurrencyId { get; set; }

        public bool IsApproved { get; set; }

        public int? CompanyId { get; set; }

        public string PostedBy { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime ApprovedDate { get; set; }
        public DateTime ApprovedDateTime { get; set; }
        public int SourceApplicationId { get; set; }
        public string SourceReferenceNumber { get; set; }
        public string JournalType { get; set; }
        public DateTime TransactionDate { get; set; }
        public double CurrencyRate { get; set; }
        public string CasaAccountNumber { get; set; }
        public int RateType { get; set; }
        public decimal Amount { get; set; }
        public int DebitGL { get; set; }
        public int CreditGL { get; set; }
    }


    public class FinancialEntriesResp
    {
        public List<TransEntries> Trans { get; set; } 
        public APIResponseStatus Status { get; set; }
    }
    public class TransEntries
    {
        public int TransactionId { get; set; }
        public string RefernceNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int SubGL { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal? RunningBal { get; set; }
        public string Description { get; set; }
    }
    public class DebitEntries
    {
        public int TransactionId { get; set; }
        public string RefernceNo { get; set; } 
        public DateTime TransactionDate { get; set; }
        public int SubGL { get; set; }
        public decimal Amount { get; set; }
        public decimal? RunningBal { get; set; }
        public string Description { get; set; }
    }
    public class TaxEntries
    {
        public int TransactionId { get; set; }
        public string RefernceNo { get; set; }

        public DateTime TransactionDate { get; set; }
        public int SubGL { get; set; }
        public decimal Amount { get; set; } 
        public decimal? RunningBal { get; set; }
        public string Description { get; set; }
    }


    public class Banks
    {
        public int BankGlId { get; set; }
        public string BankName { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string BVN { get; set; }
        public int CountryId { get; set; }
        public int CurrencyId { get; set; }
        public string BankCode { get; set; }
        public int SubGl { get; set; }
        public string SubGlName { get; set; }
    }
        public class BanksRespObj
        {
            public List<Banks> bank { get; set; }
            public APIResponseStatus Status{ get; set; }
        }

    public class ReportGls
    {
        public List<int> Trans { get; set; }   
    }
}
