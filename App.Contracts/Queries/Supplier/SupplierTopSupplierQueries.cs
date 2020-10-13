using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.Queries
{
    public class GetAllSupplierTopSupplierQuery : IRequest<SupplierTopSupplierRespObj>
    {
        public GetAllSupplierTopSupplierQuery() { }
        public int SupplierId { get; set; }
        public GetAllSupplierTopSupplierQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
     
    public class SupplierTopSupplierQuery : IRequest<SupplierTopSupplierRespObj>
    {
        public SupplierTopSupplierQuery() { }
        public int SupplierTopId { get; set; }
        public SupplierTopSupplierQuery(int supplierTopId)
        {
            SupplierTopId = supplierTopId;
        }
    }
}
