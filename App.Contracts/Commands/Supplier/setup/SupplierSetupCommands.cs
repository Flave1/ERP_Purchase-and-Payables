using MediatR;
using Puchase_and_payables.Contracts.Response.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Puchase_and_payables.Contracts.Commands.Supplier.setup
{
    public class AddUpdateSuppliertypeCommand : IRequest<SuppliertypeRegRespObj>
    {
        public int SupplierTypeId { get; set; }

        [Required]
        [StringLength(250)]
        public string SupplierTypeName { get; set; }

        public int CreditGL { get; set; }
        public int DebitGL { get; set; }    
        public int[] TaxApplicable { get; set; }

    }
    public class AddUpdateTaxsetupCommand : IRequest<TaxsetupRegRespObj>
    {
        public int TaxSetupId { get; set; }
        public string TaxName { get; set; }
        public double Percentage { get; set; }
        public string Type { get; set; }
        public int SubGL { get; set; }
    }
    public class AddUpdateServiceTermCommand : IRequest<ServiceTermRegRespObj>
    {
        public int ServiceTermsId { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
    }
}
