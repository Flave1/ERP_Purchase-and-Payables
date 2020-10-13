using FluentValidation;
using GOSLibraries.GOS_Financial_Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators.Identity
{
    public class ChangePasswordValid : AbstractValidator<ChangePassword>
    {
        public ChangePasswordValid()
        {
            RuleFor(g => g.Email).EmailAddress().NotEmpty();
            RuleFor(f => f.NewPassword).NotEmpty().MinimumLength(8);
            RuleFor(j => j.OldPassword).MinimumLength(8).NotEmpty(); 
        }
    }
}
