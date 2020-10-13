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
    public class GetFinancialDetailQueryHandler : IRequestHandler<GetFinancialDetailQuery, SupplierFinancialDetalRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        public GetFinancialDetailQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _mapper = mapper;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierFinancialDetalRespObj> Handle(GetFinancialDetailQuery request, CancellationToken cancellationToken)
        {
            var res = await _supRepo.GetFinancialDetailAsync(request.FinancialDetailId);

            var respItemList = new List<SupplierFinancialDetalObj>();
            if(res != null)
            {
                var item = new SupplierFinancialDetalObj
                {
                    Active = res.Active,
                    BusinessSize = res.BusinessSize,
                    CompanyId = res.CompanyId,
                    CreatedBy = res.CreatedBy,
                    CreatedOn = res.CreatedOn,
                    Deleted = res.Deleted,
                    FinancialdetailId = res.FinancialdetailId,
                    Year = res.Year,
                    Value = res.Value,
                    SupplierId = res.SupplierId,
                    UpdatedBy = res.UpdatedBy,
                    UpdatedOn = res.UpdatedOn,
                };
                respItemList.Add(item);
            }

            return new SupplierFinancialDetalRespObj
            {
                SupplierFinancialDetals = respItemList,
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = res == null ? "Search Complete!! No Record Found" : null
                    }
                }
            };
        }
    }
}
