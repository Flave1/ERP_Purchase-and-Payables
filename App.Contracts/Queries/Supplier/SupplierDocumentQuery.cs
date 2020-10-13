using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.Queries
{

    public class GetAllSupplierDocumentQuery : IRequest<SupplierDocumentRespObj>
    {
        public GetAllSupplierDocumentQuery() { }
        public int SupplierId { get; set; }
        public GetAllSupplierDocumentQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }

    public class GetSupplierDocumentQuery : IRequest<SupplierDocumentRespObj>
    {
        public GetSupplierDocumentQuery() { }
        public int SupplierDocumentId { get; set; }
        public GetSupplierDocumentQuery(int supplierDocumentId)
        {
            SupplierDocumentId = supplierDocumentId;
        }
    }
     
}
