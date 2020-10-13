using MediatR;
using Puchase_and_payables.Contracts.Response.Supplier;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Commands.Supplier
{
    public class AddUpdateBankAccountdetailCommand : IRequest<SupplierAccountRegRespObj>
    {
        public int BankAccountDetailId { get; set; }
        public int SupplierId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BVN { get; set; }
        public int Bank { get; set; }
    }

    public class AddUpdateSupplierFinancialDetalCommand : IRequest<SupplierFinancialDetalRegRespObj>
    {
        public int FinancialdetailId { get; set; }
        public string BusinessSize { get; set; } 
        public string Year { get; set; }
        public string Value { get; set; }
        public int SupplierId { get; set; } 
    }

    public class Deletitem : IRequest<bool>
    {
        public List<int> ItemIds { get; set; }
    }
}
