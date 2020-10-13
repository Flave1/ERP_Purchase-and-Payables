using System.Collections.Generic;

namespace Puchase_and_payables.Contracts.Response.IdentityServer
{
    namespace QuickType
    {
        using GOSLibraries.GOS_API_Response;
        using System; 
        public partial class CompanyStructureRespObj
        { 
            public List<CompanyStructure> companyStructures { get; set; } 
            public APIResponseStatus Status { get; set; }
        }

        public partial class CompanyStructure
        { 
            public long CompanyStructureId { get; set; }
             
            public string Name { get; set; }
             
            public int ParentCompanyID { get; set; }
            public string Telephone { get; set; }
            public string Address1 { get; set; }
            public object Code { get; set; }
             
            public object CountryName { get; set; }
            public int? ReportCurrencyId { get; set; }
        } 
    } 
}
