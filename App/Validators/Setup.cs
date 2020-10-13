using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Contracts.Commands.Supplier.setup;
using Puchase_and_payables.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators
{
    public class AddUpdateTaxsetupCommandVal : AbstractValidator<AddUpdateTaxsetupCommand>
    {
        private readonly DataContext _dataContext;
        public AddUpdateTaxsetupCommandVal(DataContext dataContext)
        {
            _dataContext = dataContext;

            RuleFor(d => d.TaxName)
                .NotEmpty().WithMessage("Tax Name must not be empty");
            RuleFor(d => d.Percentage).NotEmpty();
            RuleFor(d => d.SubGL).NotEmpty();
            RuleFor(d => d.Type).NotEmpty(); 
        } 
    } 
    public class AddUpdateSuppliertypeCommandVal : AbstractValidator<AddUpdateSuppliertypeCommand>
    {
        public AddUpdateSuppliertypeCommandVal()
        { 
            RuleFor(d => d.CreditGL).NotEmpty();
            RuleFor(d => d.DebitGL).NotEmpty();
            RuleFor(d => d.SupplierTypeName).NotEmpty(); 
        } 
    }
     
    public class AddUpdateServiceTermCommandVal : AbstractValidator<AddUpdateServiceTermCommand>
    {
        public AddUpdateServiceTermCommandVal()
        {
            RuleFor(d => d.Content).NotEmpty();
            RuleFor(d => d.Header).NotEmpty();
        }
    }
}
