using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Report
{
    public class DashboardCountResp
    {
        public DashboardCount DashboardCount { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class DashboardCount
    {
        public int? SupplierCount { get; set; }
        public int? PRNCount { get; set; }
        public int? BIDCount { get; set; }
        public int? LPOCount { get; set; }
        public int? PaymentsCount { get; set; }
        public int? PayablesCount { get; set; }
    }

    public class PayableDaysResp
    {
        public string[] Labels { get; set; }

        public List<PayableDays> PayableDays { get; set; } 
        public APIResponseStatus Status { get; set; }
    }

    public class SupplierData
    {
        public string SuppliertypeName { get; set; }
        public int SuppliertypeId { get; set; } 
        public int SuppliierId { get; set; } 
        public int SubGL { get; set; }
        public DateTime Date { get; set; }
        public bool InvoiceGenerated { get; set; } 
        public DateTime? CompletionDate { get; set; }
    }
    public class WinnerProposals : SupplierData
    { 
        public decimal Amount { get; set; }
    }

    public class FinalData
    {
        public string SuppliertypeName { get; set; }
        public int SuppliertypeId { get; set; } 
        public TimeSpan Date { get; set; }
    }
    public class FinalDataAnalytics
    {
        public string SuppliertypeName { get; set; }
        public int PayableDays { get; set; }
    }

    public class ProjectStatus
    {
        public int Not_Started { get; set; } 
        public int In_Progress { get; set; } 
        public int Completed { get; set; } 
        public int Cancelled { get; set; } 
    }

    public class DashboardAgeAnalysisResp
    {
        public DashboardAgeAnalysisResp()
        {
            Labels = new string[] { };

            Datasets = new List<DashbordAgeAnalysis>();
        }

        public string[] Labels { get; set; }

        public List<DashbordAgeAnalysis> Datasets { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class ProjectStatusStatsresp
    {
        public ProjectStatusStatsresp()
        {
            Labels = new string[] { };

            Datasets = new List<ProjectStatusStats>();
        }

        public string[] Labels { get; set; }

        public List<ProjectStatusStats> Datasets { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class DashbordAgeAnalysis
    {
        public string Label { get; set; }

        public decimal[] Data { get; set; }

        public bool Fill { get; set; }

        public string BorderColor { get; set; }

        public string BackgroundColor { get; set; }
    }

    public class ProjectStatusStats
    {
        public string Label { get; set; }

        public int[] Data { get; set; }

        public bool Fill { get; set; }

        public string BorderColor { get; set; }

        public string BackgroundColor { get; set; }
    }

    public class PayableDays
    {
        public string Label { get; set; }

        public decimal[] Data { get; set; }

        public bool Fill { get; set; }

        public string BorderColor { get; set; }

        public string BackgroundColor { get; set; }
    }

    public class PayableDaysTrendAnalysisResp
    {
        public PayableDaysTrendAnalysisResp()
        {
            Labels = new string[] { };

            Datasets = new List<PayableDays>();
        }

        public string[] Labels { get; set; }

        public List<PayableDays> Datasets { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
