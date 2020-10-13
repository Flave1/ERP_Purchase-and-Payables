using GODPAPIs.Contracts.RequestResponse;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using Microsoft.AspNetCore.Http;
using Puchase_and_payables.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GODPAPIs.Contracts.Commands.Supplier
{
     public class UpdateSupplierAuthorizationCommand : IRequest<SupplierAuthorizationRegRespObj>
    {
        public int SupplierAuthorizationId { get; set; } 
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; } 
        public IFormFile Signature { get; set; } 
        public bool? Active { get; set; } 
        public bool? Deleted { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTime? CreatedOn { get; set; } 
        public string UpdatedBy { get; set; } 
        public DateTime? UpdatedOn { get; set; }  
    }

    public class DeleteSupplierAuthorizationCommand : IRequest<DeleteRespObj>
    {
        public List<DeleteItemReqObj> req { get; set; }
    }
}
