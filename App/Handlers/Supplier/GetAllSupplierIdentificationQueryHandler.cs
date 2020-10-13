using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Response.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier
{ 
    public class GetAllSupplierIdentificationQueryHandler : IRequestHandler<GetAllSupplierIdentificationQuery, IdentificationRespObj>
    {
        private readonly ISupplierRepository _supRepo; 
        public GetAllSupplierIdentificationQueryHandler(ISupplierRepository supplierRepository)
        { 
            _supRepo = supplierRepository;
        }
        public async Task<IdentificationRespObj> Handle(GetAllSupplierIdentificationQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _supRepo.GetAllSupplierIdentificationAsync(request.SupplierId);
            return new  IdentificationRespObj
            {
                Indentifications = supplier.Select(d => new IdentificationObj
                {
                    BusinessType = d.BusinessType,
                    SupplierId = d.SupplierId,
                    BusinessTypeName = Convert.ToString((BusinessType)d.BusinessType),
                    ExpiryDate = d.Expiry_Date,
                    HaveWorkPermit = d.HaveWorkPermit,
                    Identification = d.Identification,
                    IdentificationName  = Convert.ToString((Identification)d.Identification),
                    IdentificationNumber = d.Identification_Number,
                    IdentificationId = d.IdentificationId,
                    IsCorporate = d.IsCorporate,
                    IncorporationDate = d.IncorporationDate,
                    Nationality = d.Nationality,
                    NationalityName = Convert.ToString(d.Nationality),
                    OtherBusinessType = d.OtherBusinessType,
                    Registrationnumber = d.RegistrationNumber
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
