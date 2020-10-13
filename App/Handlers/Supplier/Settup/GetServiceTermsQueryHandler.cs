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
    public class GetServiceTermsQueryHandler : IRequestHandler<GetServiceTermsQuery, ServiceTermRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        public GetServiceTermsQueryHandler(ISupplierRepository supplierRepository)
        {
            _supRepo = supplierRepository;
        }
        public async Task<ServiceTermRespObj> Handle(GetServiceTermsQuery request, CancellationToken cancellationToken)
        {
            var item = await _supRepo.GetServiceTermsAsync(request.ServiceTermId);

            var respList = new List<ServiceTermObj>();
            if (item != null)
            {
                var respItem = new ServiceTermObj
                {
                    CompanyId = item.CompanyId,
                    Active = item.Active,
                    CreatedBy = item.CreatedBy,
                    CreatedOn = item.CreatedOn,  
                    ServiceTermsId = item.ServiceTermsId, 
                    UpdatedBy = item.UpdatedBy,
                    UpdatedOn = item.UpdatedOn,
                    Content = item.Content,
                    Header = item.Header
                };
                respList.Add(respItem);
            }

            return new ServiceTermRespObj
            {
                ServiceTerms = respList,
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = respList.Count() > 0 ? null : "Search Complete!! No Record Found"
                    }
                }
            };
        }
    }
}
