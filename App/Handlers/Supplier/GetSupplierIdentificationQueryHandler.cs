using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.DomainObjects.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier
{ 

    public class GetSupplierIdentificationQueryHandler : IRequestHandler<GetSupplierIdentificationQuery, IdentificationRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        public GetSupplierIdentificationQueryHandler(
            ISupplierRepository supplierRepository,
            IMapper mapper)
        { 
            _supRepo = supplierRepository;
            _mapper = mapper;
        }
        public async Task<IdentificationRespObj> Handle(GetSupplierIdentificationQuery request, CancellationToken cancellationToken)
        {

            var supplier = await _supRepo.GetSupplierIdentificationAsync(request.IdentityId);
            var itemList = new List<cor_identification>();
            itemList.Add(supplier);
            return new IdentificationRespObj
            {
                Indentifications = _mapper.Map<List<IdentificationObj>>(itemList),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = supplier == null ? "Search Complete!! No Record Found" : null
                    }
                }
            };
        }
    }
}
