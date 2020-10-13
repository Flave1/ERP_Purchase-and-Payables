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
    public class UpdateSupplierDocumentCommand : IRequest<SupplierDocumentRegRespObj>
    {
        public int SupplierDocumentId { get; set; }
        public int SupplierId { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public IFormFile Document { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
    }
    public class DeleteSupplierDocumentCommand : IRequest<DeleteRespObj>
    {
        public List<DeleteItemReqObj> req { get; set; }
    }
}
