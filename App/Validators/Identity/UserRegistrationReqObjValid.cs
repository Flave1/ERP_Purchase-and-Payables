using FluentValidation;
using GOSLibraries.GOS_Financial_Identity;
using Puchase_and_payables.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators.Identity
{
    public class UserRegistrationReqObjValid : AbstractValidator<CustomUserRegistrationReqObj>
    {
        public UserRegistrationReqObjValid()
        {
            RuleFor(d => d.Email).EmailAddress().NotEmpty();
            RuleFor(d => d.FulName).NotEmpty().MinimumLength(3);
            RuleFor(d => d.MobileNumber).NotEmpty().MinimumLength(11);
            RuleFor(d => d.Password).NotEmpty();
        }
    }
}
