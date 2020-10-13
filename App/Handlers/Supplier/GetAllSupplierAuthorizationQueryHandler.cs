using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.GOS_API_Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class GetAllSupplierAuthorizationQueryHandler : IRequestHandler<GetAllSupplierAuthorizationQuery, SupplierAuthorizationRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        public GetAllSupplierAuthorizationQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierAuthorizationRespObj> Handle(GetAllSupplierAuthorizationQuery request, CancellationToken cancellationToken)
        {
            var list = await _supRepo.GetAllSupplierAuthorizationAsync(request.SupplierId);
             

            return new SupplierAuthorizationRespObj
            {
                SupplierAuthorizations = list.Select(x => new SupplierAuthorizationObj() {
                    UpdatedOn = x.UpdatedOn,
                    Active = x.Active,
                    Address = x?.Address ?? string.Empty,
                    CreatedBy = x.CreatedBy ?? string.Empty,
                    CreatedOn = x.CreatedOn,
                    Deleted = x.Deleted,
                    Email = x.Email ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    PhoneNo = x.PhoneNo ?? string.Empty,
                    Signature = x.Signature ?? null,
                    SupplierAuthorizationId = x.SupplierAuthorizationId,
                    SupplierId = x.SupplierId,
                    UpdatedBy = x.UpdatedBy
                }).ToList(),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = list.Count() > 0 ? null: "Search Complete!! No Record Found" 
                    }
                }
            };
        }
    }
}
