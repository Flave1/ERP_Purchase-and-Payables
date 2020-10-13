using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.RequestResponse.Supplier
{
    public class SupplierTypeObj
    { 
        public int SupplierTypeId { get; set; } 
        public string SupplierTypeName { get; set; }
        public int[] TaxApplicable { get; set; }
        public string TaxApplicableName { get; set; }
        public bool? Active { get; set; } 
        public bool? Deleted { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTime? CreatedOn { get; set; } 
        public string UpdatedBy { get; set; } 
        public DateTime? UpdatedOn { get; set; }
        public int ExcelLineNumber { get; set; }
    }
}
