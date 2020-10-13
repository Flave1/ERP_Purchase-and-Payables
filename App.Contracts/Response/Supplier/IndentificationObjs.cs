using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Response.Supplier
{
    public class IdentificationObj
    {
        public int IdentificationId { get; set; }
        public int SupplierId { get; set; }
        public bool IsCorporate { get; set; }
        public string Registrationnumber { get; set; }
        public DateTime? IncorporationDate { get; set; }
        public int BusinessType { get; set; }
        public string BusinessTypeName { get; set; }
        public string OtherBusinessType { get; set; }

        public int Identification { get; set; }
        public string IdentificationName { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Nationality { get; set; }
        public string NationalityName { get; set; }
        public bool HaveWorkPermit { get; set; }
    }

    public class IdentificationRespObj
    {
        public List<IdentificationObj> Indentifications { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class IdentificationRegRespObj
    {
        public int IdentityId { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
