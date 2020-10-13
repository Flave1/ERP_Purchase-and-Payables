using FluentValidation;
using GOSLibraries.GOS_Financial_Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators.Identity
{
    public class ConfirnmationRequestValid : AbstractValidator<ConfirnmationRequest>
    {
        public ConfirnmationRequestValid()
        {
            RuleFor(f => f.Email).EmailAddress().NotEmpty();
            RuleFor(h => h.Code).MinimumLength(5).WithMessage("Invalid Verification Code").NotEmpty();
        }
    }
}
