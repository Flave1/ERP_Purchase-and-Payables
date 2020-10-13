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
    public class GetAllSupplierTopSupplierQueryHandler : IRequestHandler<GetAllSupplierTopSupplierQuery, SupplierTopSupplierRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        public GetAllSupplierTopSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierTopSupplierRespObj> Handle(GetAllSupplierTopSupplierQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _supRepo.GetAllSupplierTopSupplierAsync(request.SupplierId); 
            return new SupplierTopSupplierRespObj
            {
                TopSuppliers = supplier.Select(d => new SupplierTopSupplierObj
                {
                    NoOfStaff = d.NoOfStaff,
                    Name = d.Name,
                    Email = d.Email,
                    Active = d.Active,
                    Address = d.Address,
                    ContactPerson = d.ContactPerson,
                    CreatedBy = d.CreatedBy,
                    CreatedOn = d.CreatedOn,
                    Deleted = d.Deleted,
                    PhoneNo = d.PhoneNo,
                    SupplierId = d.SupplierId,
                    TopSupplierId = d.TopSupplierId,
                     UpdatedBy = d.UpdatedBy,
                     UpdatedOn  = d.UpdatedOn,
                        
                }).ToList(),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = supplier.Count() > 0 ? null : "Search Complete!! No Record Found"
                    }
                }
            };
        }
    }
}
