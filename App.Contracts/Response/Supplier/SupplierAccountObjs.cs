using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Supplier
{
    public class SupplierBankAccountDetailsObj : GeneralEntity
    {
        public int BankAccountDetailId { get; set; }
        public int SupplierId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BVN { get; set; }
        public int Bank { get; set; }
        public string BankName { get; set; }
    }
    public class SupplierAccountRespObj
    {
        public List<SupplierBankAccountDetailsObj> SupplierAccountBankDetails { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class SupplierAccountRegRespObj
    {
        public int BankAccountDetailId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class SupplierFinancialDetalObj : GeneralEntity
    {
        public int FinancialdetailId { get; set; }
        public string BusinessSize { get; set; }
        public string Year { get; set; }
        public string Value { get; set; } 
        public int SupplierId { get; set; }
    }
    public class SupplierFinancialDetalRespObj
    {
        public List<SupplierFinancialDetalObj> SupplierFinancialDetals  { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class SupplierFinancialDetalRegRespObj
    {
        public int FinancialdetailId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
