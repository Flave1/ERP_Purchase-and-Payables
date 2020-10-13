using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
