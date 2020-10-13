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

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class GetSupplierAuthorizationHandler : IRequestHandler<GetSupplierAuthorization, SupplierAuthorizationRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        public GetSupplierAuthorizationHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierAuthorizationRespObj> Handle(GetSupplierAuthorization request, CancellationToken cancellationToken)
        {
            var item = await _supRepo.GetSupplierAuthorizationAsync(request.SupplierAuthorizationId);
            var itemRespList = new List<SupplierAuthorizationObj>();
            if(item != null)
            {
                var itemResp = new SupplierAuthorizationObj
                {
                    Active = item.Active,
                    Address = item?.Address ?? string.Empty,
                    CreatedBy = item?.CreatedBy ?? string.Empty,
                    CreatedOn = item?.CreatedOn,
                    Deleted = item.Deleted,
                    Email = item?.Email ?? string.Empty,
                    Name = item?.Name ?? string.Empty,
                    PhoneNo = item?.PhoneNo ?? string.Empty,
                    Signature = item?.Signature ?? null,
                    SupplierAuthorizationId = item.SupplierAuthorizationId,
                    SupplierId = item.SupplierId,
                    UpdatedBy = item?.UpdatedBy,
                    UpdatedOn = item.UpdatedOn,
                };
                itemRespList.Add(itemResp);
            }
            return new SupplierAuthorizationRespObj
            {
                SupplierAuthorizations = itemRespList,
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = item == null ? "Search Complete!! No Record Found" : null
                    }
                }
            };
        }
    }
}
