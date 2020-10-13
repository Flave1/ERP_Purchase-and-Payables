using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Queries.Supplier;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup
{
   
    public class GetTaxsetupQueryHandler : IRequestHandler<GetTaxSetupQuery, TaxsetupRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        public GetTaxsetupQueryHandler(ISupplierRepository supplierRepository)
        {
            _supRepo = supplierRepository;
        }
        public async Task<TaxsetupRespObj> Handle(GetTaxSetupQuery request, CancellationToken cancellationToken)
        {
            var item = await _supRepo.GetTaxSetupAsync(request.TaxSetupId);

            var respList = new List<TaxsetupObj>();
            if(item != null)
            {
                var respItem = new TaxsetupObj
                {
                    CompanyId = item.CompanyId,
                    Active = item.Active,
                    CreatedBy = item.CreatedBy,
                    CreatedOn = item.CreatedOn,
                    Percentage = item.Percentage,
                    SubGL = item.SubGL,
                    TaxSetupId = item.TaxSetupId,
                    Type = item.Type,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedOn = item.UpdatedOn,
                    TaxName = item.TaxName,
                };
                respList.Add(respItem);
            }

            return new TaxsetupRespObj
            {
                TasxSetups = respList,
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
