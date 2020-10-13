using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Queries.Supplier;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Settup
{

    public class GetAllSupplierTypeQueryHandler : IRequestHandler<GetAllSupplierTypeQuery, SuppliertypeRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IFinanceServerRequest _financeServer;
        public GetAllSupplierTypeQueryHandler(
            ISupplierRepository supplierRepository,
            IFinanceServerRequest serverRequest)
        {
            _financeServer = serverRequest;
            _supRepo = supplierRepository;
        }
        public async Task<SuppliertypeRespObj> Handle(GetAllSupplierTypeQuery request, CancellationToken cancellationToken)
        {
            var list = await _supRepo.GetAllSupplierTypeAsync();
            var Tax = await _supRepo.GetAllTaxSetupAsync();
            var gls = await _financeServer.GetAllSubglAsync();
            var respList = new List<SuppliertypeObj>();
            respList = list.Select(x => new SuppliertypeObj()
            {
                UpdatedOn = x.UpdatedOn,
                Active = x.Active,
                CreatedBy = x.CreatedBy ?? string.Empty,
                CreatedOn = x.CreatedOn,
                Deleted = x.Deleted,
                UpdatedBy = x.UpdatedBy,
                CreditGL = x.CreditGL,
                DebitGL = x.DebitGL,
                SupplierTypeName = x.SupplierTypeName,
                SupplierTypeId = x.SupplierTypeId,
                TaxApplicable = x.TaxApplicable.Split(',').Select(int.Parse).ToArray(), 
                
            }).ToList();

            if(respList.Count() > 0)
            {
                foreach(var item in respList)
                {
                    item.TaxApplicableName = string.Join(", ", Tax.Where(d => item.TaxApplicable.Contains(d.TaxSetupId)).Select(w => w.TaxName));
                    item.CreditSubGlCode = gls.SubGls.FirstOrDefault(q => q.subGLId == item.CreditGL)?.subGLCode;
                    item.DebitSubGlCode = gls.SubGls.FirstOrDefault(q => q.subGLId == item.DebitGL)?.subGLCode;
                }
            }

            return new SuppliertypeRespObj
            {
                Suppliertypes = respList,
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = list.Count() > 0 ? null : "Search Complete!! No Record Found"
                    }
                }
            };
        }
    }
}
