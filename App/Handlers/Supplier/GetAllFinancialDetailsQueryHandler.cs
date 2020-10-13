using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
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
  
    
        public class GetAllFinancialDetailsQueryHandler : IRequestHandler<GetAllFinancialDetailsQuery, SupplierFinancialDetalRespObj>
        {
            private readonly ISupplierRepository _supRepo;
            private readonly IMapper _mapper;
            public GetAllFinancialDetailsQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
            {
                _mapper = mapper;
                _supRepo = supplierRepository;
            }
            public async Task<SupplierFinancialDetalRespObj> Handle(GetAllFinancialDetailsQuery request, CancellationToken cancellationToken)
            {
                var resp =  _supRepo.GetAllFinancialDetails(request.SupplierId);
                return new SupplierFinancialDetalRespObj
                {
                    SupplierFinancialDetals = resp.Select(d => new SupplierFinancialDetalObj() { 
                        UpdatedOn = d.UpdatedOn,
                        UpdatedBy = d.UpdatedBy,
                        SupplierId = d.SupplierId,
                        Year = d.Year,
                        Value = d.Value, 
                        FinancialdetailId = d.FinancialdetailId,
                        Deleted = d.Deleted,
                        CreatedOn = d.CreatedOn,
                        Active = d.Active,
                        BusinessSize = d.BusinessSize,
                        CompanyId = d.CompanyId,
                        CreatedBy = d.CreatedBy, 
                    }).ToList(),
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = resp == null ? "Search Complete!! No Record Found" : null
                        }
                    }
                };
            }
        }
    }
