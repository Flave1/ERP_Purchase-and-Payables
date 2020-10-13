using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.IdentityServer
{
    public partial class CommonLookup
    { 
        public int LookupId { get; set; }
         
        public int ParentId { get; set; }
         
        public string LookupName { get; set; }
          
        public int SellingRate { get; set; }
         
        public int BuyingRate { get; set; }
         
        public bool BaseCurrency { get; set; }
         
        public string Date { get; set; }
         
        public int CorporateChargeAmount { get; set; }
         
        public int IndividualChargeAmount { get; set; }
         
        public int GlAccountId { get; set; }   
        public string code { get; set; }


    }

    public class CommonLookupRespObj
    {
        public List<CommonLookup> commonLookups { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class ActivityRespObj
    {
        public List<ActivityObj> Activities { get; set; }
        public APIResponseStatus Status { get; set; }
    }
    public class ActivityObj
    {
        public int ActivityId { get; set; }
        public int ActivityParentId { get; set; }
        public string ActivityName { get; set; }
        //.......................
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string ActivityParentName { get; set; }
        public bool CanAdd { get; set; }
        public bool CanApprove { get; set; }
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
        //........................ 
    }

   

    public class UserRoleObj
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }

    }


    public class UserRoleRespObj
    {
        public List<UserRoleObj> UserRoles { get; set; }
        public List<ActivityObj> UserRoleActivities { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
