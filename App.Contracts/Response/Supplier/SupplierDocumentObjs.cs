using GOSLibraries.GOS_API_Response; 
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.RequestResponse.Supplier
{
    public class SupplierDocumentObj
    { 
        public int SupplierDocumentId { get; set; } 
        public int SupplierId { get; set; } 
        public string Name { get; set; }
        public int DocumentId { get; set; }
        public byte[] Document { get; set; } 
        public bool? Active { get; set; } 
        public bool? Deleted { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTime? CreatedOn { get; set; } 
        public string UpdatedBy { get; set; } 
        public DateTime? UpdatedOn { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
    }
    public class SupplierDocumentRegRespObj 
    {
        public int SupplierDocumentId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class SupplierDocumentRespObj
    {
        public List<SupplierDocumentObj> SupplierDocument { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
