using GOSLibraries.GOS_API_Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Report
{
    public class PurchAndPayaReport
    {
        public string SupplierName { get; set; }
        public int SupplierId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } 
        public DateTime ExpectedDeleiveryDate { get; set; }
        public DateTime? ActualDeleiverDate { get; set; }
        public string DeleiveryLocation { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? OutStandingAmount { get; set; }
        public long? PayableDays { get; set; }
        public int? RequestProcessPeriod { get; set; }

        public int LPOId { get; set; }

        public string LPONumber { get; set; }
        public string Description { get; set; }
        public DateTime? RequisitionDate { get; set; }
        public int? Department { get; set; } 
        public string SupplierLocation { get; set; } 
        public decimal? AmountPayable { get; set; } 
        public DateTime? RequestDate { get; set; }
        //public string Title { get; set; }
        //public string SubStrucure { get; set; }
        //public DateTime ReportDate { get; set; }
        //public string From { get; set; }
        //public string To { get; set; }
        //public int NumberOfLPO { get; set; }
        //public decimal Total { get; set; }
        // public List<LPOReport> LPOReports { get; set; }
    }
   
    public class PurchAndPayaReportResp
    {
        public List<PurchAndPayaReport> PurchAndsPayables { get; set; }
        public APIResponseStatus Staus { get; set; }
    }

    public class PurchaseAndPayablesReportQuery : IRequest<PurchAndPayaReportResp>
    {
        public string LPONumber { get; set; }
        public string ItemDescription { get; set; }
        public DateTime? RequisitionDate { get; set; }
        public int? Department { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierLocation { get; set; }
        public DateTime? ExpectedDeleiveryDate { get; set; }
        public DateTime? ActualDeleiverDate { get; set; }
        public string DeleiveryLocation { get; set; }
        public decimal? AmountPayable { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? OutStandingAmount { get; set; }
        public long? PayableDays { get; set; }
        public int? RequestProcessPeriod { get; set; }

    }

}
