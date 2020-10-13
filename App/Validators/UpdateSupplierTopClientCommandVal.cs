using FluentValidation;
using GODPAPIs.Contracts.Commands.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Validations
{
    public class UpdateSupplierTopClientCommandVal : AbstractValidator<UpdateSupplierTopClientCommand>
    {
        public UpdateSupplierTopClientCommandVal()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.PhoneNo).NotEmpty().MinimumLength(11);
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.ContactPerson).NotEmpty();
            RuleFor(x => x.NoOfStaff).NotEmpty(); 
            RuleFor(s => s.CountryId).NotEmpty();
        }
    }
}
