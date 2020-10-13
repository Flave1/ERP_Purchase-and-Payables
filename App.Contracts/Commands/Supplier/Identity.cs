using GOSLibraries.GOS_Financial_Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Commands.Supplier
{
    public class RegistrationCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string FulName { get; set; }
        public string Password { get; set; }
    }
}
