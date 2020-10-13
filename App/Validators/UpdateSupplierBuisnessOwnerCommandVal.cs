using FluentValidation;
using GODPAPIs.Contracts.Commands.Supplier;
using Puchase_and_payables.Contracts.Commands.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Validations
{
    public class UpdateSupplierBuisnessOwnerCommandVal : AbstractValidator<UpdateSupplierBuisnessOwnerCommand>
    {
        public UpdateSupplierBuisnessOwnerCommandVal()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.PhoneNo).NotEmpty().MinimumLength(11);
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
        }
    }

    public class AddUpdateSupplierFinancialDetalCommandVal : AbstractValidator<AddUpdateSupplierFinancialDetalCommand>
    {
        public AddUpdateSupplierFinancialDetalCommandVal()
        {
            RuleFor(s => s.BusinessSize).NotEmpty();
            RuleFor(s => s.SupplierId).NotEmpty().WithMessage("Unable to Identify Supplier");
            RuleFor(w => w.Value).NotEmpty();
            RuleFor(w => w.Year).NotEmpty(); 
        }
    }
}
