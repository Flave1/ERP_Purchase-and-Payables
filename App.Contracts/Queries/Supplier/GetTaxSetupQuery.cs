using MediatR;
using Puchase_and_payables.Contracts.Response.Supplier; 

namespace Puchase_and_payables.Contracts.Queries.Supplier
{
    public class GetTaxSetupQuery : IRequest<TaxsetupRespObj>
    {
        public GetTaxSetupQuery() { }
        public int TaxSetupId { get; set; }
        public GetTaxSetupQuery(int TaxSetupId)
        {
            TaxSetupId = TaxSetupId;
        }
    }
    public class GetServiceTermsQuery : IRequest<ServiceTermRespObj>
    {
        public GetServiceTermsQuery() { }
        public int ServiceTermId { get; set; }
        public GetServiceTermsQuery(int serviceTermId)
        {
            ServiceTermId = serviceTermId;
        }
    }
    public class GetSupplierTypeQuery : IRequest<SuppliertypeRespObj>
    {
        public GetSupplierTypeQuery() { }
        public int SupplierTypeId { get; set; }
        public GetSupplierTypeQuery(int supplierTypeId)
        {
            SupplierTypeId = supplierTypeId;
        }
    }

    public class GetAllTaxSetupQuery : IRequest<TaxsetupRespObj> { }
    public class GetAllServiceTermsQuery : IRequest<ServiceTermRespObj> { }
    public class GetAllSupplierTypeQuery : IRequest<SuppliertypeRespObj> { }
}
