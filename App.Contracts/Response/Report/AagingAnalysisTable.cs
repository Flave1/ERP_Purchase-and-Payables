using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Report
{
    public class AagingAnalysisTable
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string SupplierNumber { get; set; }
        public string SupplierName { get; set; }
        public int SupplierId { get; set; }
        public string ItemDescription { get; set; }
        public decimal Day_0_30 { get; set; }
        public decimal Day_31_60 { get; set; } 
        public decimal Day_61_90 { get; set; }
        public decimal Day_91_180 { get; set; }
        public decimal Day_180_Above { get; set; }
        public int Total_Days { get; set; }
    }
    public class AgingAnalysisRespObj
    {
        public List<AagingAnalysisTable> AagingAnalysis { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class AagingAnalysisGraph
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string SupplierNumber { get; set; }
        public string SupplierName { get; set; }
        public int SupplierId { get; set; }
        public string ItemDescription { get; set; }
        public decimal Day_0_30 { get; set; }
        public decimal Day_31_60 { get; set; }
        public decimal Day_61_90 { get; set; }
        public decimal Day_91_180 { get; set; }
        public decimal Day_180_Above { get; set; }
        public int Total_Days { get; set; }
    }
    public class DashbordAgingAnalysisRespObj
    {
        public List<AagingAnalysisGraph> AagingAnalysis { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
