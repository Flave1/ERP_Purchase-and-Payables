using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.IdentityServer
{ 

    public class EmailAddressObj
    {
        public string Name { get; set; }
        public string Address { get; set; }

    } 
    public class EmailMessageObj
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<EmailAddressObj> ToAddresses { get; set; }
        public List<EmailAddressObj> FromAddresses { get; set; }
        public bool SendIt { get; set; }
        public bool SaveIt { get; set; }
        public int Template { get; set; }
        public List<int> ActivitIds { get; set; }
    }
    public class SendEmailRespObj
    {
        public int ResponseStatus { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
