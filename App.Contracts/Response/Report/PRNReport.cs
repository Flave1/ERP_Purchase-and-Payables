using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Report
{
    public class PRNReport
    {
        public string Description { get; set; }
        public string RefrenceNumber { get; set; }
        public DateTime DateOfRequest { get; set; }
        public string RequestBy { get; set; } 
        public decimal Total { get; set; }
        public List<LPOReport> LPOReports { get; set; }
    }

    public class LPOReport
    {
        public string LPONumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string ServiceTerms { get; set; }
        public decimal AmountPayable { get; set; }
        public int Quantity { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string Description { get; set; }
        public string RequestBy { get; set; }
        public decimal Tax { get; set; }
        public From From { get; set; }
        public To To { get; set; }
        public List<Phases> Phases { get; set; }
        public ServiceTerm ServiceTerm { get; set; }
        public APIResponseStatus Status { get; set; }
         
    }
    public class Phases
    {
        public int Phase { get; set; } 
        public decimal Amount { get; set; }
        public double Tax { get; set; } 
    }

    public class From
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Address { get; set; }
    }

    public class To
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Address { get; set; }
    }

    public class ServiceTerm
    {
        public string Header { get; set; }
        public string Content { get; set; }
    }
   
}
