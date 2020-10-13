using GOSLibraries.GOS_API_Response; 
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.RequestResponse.Supplier
{
    public class SupplierAuthorizationObj
    { 
        public int SupplierAuthorizationId { get; set; } 
        public int SupplierId { get; set; } 
        public string Name { get; set; } 
        public string Address { get; set; } 
        public string Email { get; set; } 
        public string PhoneNo { get; set; } 
        public byte[] Signature { get; set; } 
        public bool? Active { get; set; } 
        public bool? Deleted { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTime? CreatedOn { get; set; } 
        public string UpdatedBy { get; set; } 
        public DateTime? UpdatedOn { get; set; } 
    }

    public class SupplierAuthorizationRegRespObj
    {
        public int SupplierAuthorizationId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class SupplierAuthorizationRespObj
    {
        public List<SupplierAuthorizationObj> SupplierAuthorizations { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
