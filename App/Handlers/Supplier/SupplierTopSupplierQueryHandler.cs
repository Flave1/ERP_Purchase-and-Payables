using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries; 
using GODPAPIs.Contracts.RequestResponse.Supplier;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GOSLibraries.GOS_API_Response;
using GODP.APIsContinuation.DomainObjects.Supplier;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class SupplierTopSupplierQueryHandler : IRequestHandler<SupplierTopSupplierQuery, SupplierTopSupplierRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        public SupplierTopSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierTopSupplierRespObj> Handle(SupplierTopSupplierQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _supRepo.GetSupplierTopSupplierAsync(request.SupplierTopId);
            var itemList = new List<cor_topsupplier>();
            itemList.Add(supplier);
            return new SupplierTopSupplierRespObj   
            {
                TopSuppliers = _mapper.Map<List<SupplierTopSupplierObj>>(itemList),
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
