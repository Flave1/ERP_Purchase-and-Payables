using GOSLibraries.GOS_API_Response; 
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.RequestResponse.Supplier
{
    public class SupplierTopSupplierObj
    { 
        public int TopSupplierId { get; set; } 
        public int SupplierId { get; set; } 
        public string Name { get; set; } 
        public string Address { get; set; } 
        public string Email { get; set; } 
        public string PhoneNo { get; set; } 
        public string ContactPerson { get; set; } 
        public int? NoOfStaff { get; set; } 
        public bool? Active { get; set; } 
        public bool? Deleted { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTime? CreatedOn { get; set; } 
        public string UpdatedBy { get; set; } 
        public DateTime? UpdatedOn { get; set; } 
    }
    public class SupplierTopSupplierRegRespObj
    {
        public int TopSupplierId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class SupplierTopSupplierRespObj
    {
        public List<SupplierTopSupplierObj> TopSuppliers { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
