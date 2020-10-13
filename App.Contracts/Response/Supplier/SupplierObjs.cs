using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Contracts.Response.Supplier;
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.RequestResponse.Supplier
{
    public class SupplierObj : SupplierBankAccountDetailsObj
    {
        public int SupplierTypeId { get; set; }
        public string Name { get; set; }
        public string Passport { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string RegistrationNo { get; set; }
        public int CountryId { get; set; }
        public int ApprovalStatusId { get; set; }
        public string Website { get; set; }
        public string TaxIDorVATID { get; set; }
        public string PostalAddress { get; set; }
        public string SupplierNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool HaveWorkPrintPermit { get; set; }
        public string SupplierTypeName { get; set; }
        public string StatusName { get; set; }
        public string WorkflowToken { get; set; }
        public int Particulars { get; set; }
         

        public int ExcelLineNumber { get; set; }
        public string WorkPermitName { get; set; }
        public string ParticularsName { get; set; }
        public string CountryName { get; set; }
    }
    public class SupplierRegRespObj
    {
        public int SupplierId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class SupplierRespObj
    {
        public List<SupplierObj> Suppliers { get; set; }
        public APIResponseStatus Status { get; set; }
    } 
}
