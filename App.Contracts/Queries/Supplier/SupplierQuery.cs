using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Supplier;
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.Queries
{
    public class GetAllSupplierQuery : IRequest<SupplierRespObj> { }
    public class GetSupplierQuery : IRequest<SupplierRespObj>
    {
        public GetSupplierQuery() { }
        public int SupplierId { get; set; }
        public GetSupplierQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
   

    public class GetBankAccountdetailQuery : IRequest<SupplierAccountRespObj>
    {
        public GetBankAccountdetailQuery() { }
        public int BankAccountDetailId { get; set; }
        public GetBankAccountdetailQuery(int bankAccountDetailId)
        {
            BankAccountDetailId = bankAccountDetailId;
        }
    }
    public class GetAllBankAccountdetailQuery : IRequest<SupplierAccountRespObj>
    {
        public GetAllBankAccountdetailQuery() { }
        public int SupplierId { get; set; }
        public GetAllBankAccountdetailQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }


    public class GetAllFinancialDetailsQuery : IRequest<SupplierFinancialDetalRespObj>
    {
        public GetAllFinancialDetailsQuery() { }
        public int SupplierId { get; set; }
        public GetAllFinancialDetailsQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }

    public class GetFinancialDetailQuery : IRequest<SupplierFinancialDetalRespObj> {
        public GetFinancialDetailQuery() { }
        public int FinancialDetailId { get; set; }
        public GetFinancialDetailQuery(int financialDetailId)
        {
            FinancialDetailId = financialDetailId;
        }
    }

    public class GetListOfApprovedSuppliersQuery : IRequest<SupplierRespObj> { }
    public class GetSuppliersByPRNQUery : IRequest<SupplierRespObj>
    {
        public GetSuppliersByPRNQUery() { }
        public int PRNId { get; set; }
        public GetSuppliersByPRNQUery(int prnId)
        {
            PRNId = prnId;
        }
    }
    

    public class GetAllSupplierIdentificationQuery : IRequest<IdentificationRespObj>
    {
        public GetAllSupplierIdentificationQuery() { }
        public int SupplierId { get; set; }
        public GetAllSupplierIdentificationQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }

    public class GetSupplierIdentificationQuery : IRequest<IdentificationRespObj>
    {
        public GetSupplierIdentificationQuery() { }
        public int IdentityId { get; set; }
        public GetSupplierIdentificationQuery(int identityId)
        {
            IdentityId = identityId;
        }
    }
}
