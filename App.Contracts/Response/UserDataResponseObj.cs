using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text; 

namespace Puchase_and_payables.Contracts.Response
{
    public class UserDataResponseObj
    {
        public int StaffId { get; set; }
        public int CompanyId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } 
        public string CustomerName { get; set; } 
        public string PhoneNumber { get; set; }
        public int SupplierId { get; set; }
        public string StaffName { get; set; } 
        public int DepartmentId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class CustomUserRegistrationReqObj
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string FulName { get; set; }
        public string Password { get; set; }
    }
}
