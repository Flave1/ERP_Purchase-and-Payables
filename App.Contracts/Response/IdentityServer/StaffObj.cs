using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.IdentityServer
{
    public class StaffObj
    {
        public int staffId { get; set; }
        public string staffCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public int jobTitle { get; set; }
        public string Email { get; set; }
    }
    public class StaffRespObj
    {
        public List<StaffObj> staff { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
