using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Queries.Supplier;
using Puchase_and_payables.Contracts.Response.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup
{
    public class GetAllServiceTermsQueryHandler : IRequestHandler<GetAllServiceTermsQuery, ServiceTermRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        public GetAllServiceTermsQueryHandler(ISupplierRepository supplierRepository)
        {
            _supRepo = supplierRepository;
        }
        public async Task<ServiceTermRespObj> Handle(GetAllServiceTermsQuery request, CancellationToken cancellationToken)
        {
            var list = await _supRepo.GetAllServiceTermsAsync();


            return new ServiceTermRespObj
            {
                ServiceTerms = list.Select(x => new ServiceTermObj()
                {
                    UpdatedOn = x.UpdatedOn,
                    Active = x.Active,
                    CreatedBy = x.CreatedBy ?? string.Empty,
                    CreatedOn = x.CreatedOn,
                    Deleted = x.Deleted,
                    ServiceTermsId = x.ServiceTermsId,
                    UpdatedBy = x.UpdatedBy,
                    CompanyId = x.CompanyId,
                    Content  = x.Content,
                    Header = x.Header
                }).ToList(),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = list.Count() > 0 ? null : "Search Complete!! No Record Found"
                    }
                }
            };
        }
    }
}
