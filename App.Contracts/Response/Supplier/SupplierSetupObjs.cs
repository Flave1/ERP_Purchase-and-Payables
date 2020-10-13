using GOSLibraries.GOS_API_Response;
using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Supplier
{
    public class ServiceTermObj : GeneralEntity
    {
        public int ServiceTermsId { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public int ExcelLineNumber { get; set; }
    }
    public class ServiceTermRespObj
    {
        public List<ServiceTermObj> ServiceTerms { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class ServiceTermRegRespObj
    {
        public int ServiceTermId { get; set; }
        public APIResponseStatus Status { get; set; }
    }


    public class TaxsetupObj : GeneralEntity
    {
        public int TaxSetupId { get; set; }
        public string TaxName { get; set; }
        public double Percentage { get; set; }
        public string Type { get; set; }
        public int SubGL { get; set; }
        public string SubGlName { get; set; }
        public int ExcelLineNumber { get; set; }
    }
    public class TaxsetupRegRespObj
    {
        public int TaxSetupId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class TaxsetupRespObj
    {
        public List<TaxsetupObj> TasxSetups { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public partial class SuppliertypeObj : GeneralEntity
    {
        public int SupplierTypeId { get; set; }

        [Required]
        [StringLength(250)]
        public string SupplierTypeName { get; set; }

        public int CreditGL { get; set; }
        public int DebitGL { get; set; }
        public int[] TaxApplicable { get; set; }
        public string TaxApplicableName { get; set; }
        public string CreditSubGlCode { get; set; }
        public string DebitSubGlCode { get; set; }

    } 
    public class SuppliertypeRegRespObj
    {
        public int SuppliertypeId { get; set; }
        public APIResponseStatus Status { get; set; }
    } 
    public class SuppliertypeRespObj
    {
        public List<SuppliertypeObj> Suppliertypes { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
