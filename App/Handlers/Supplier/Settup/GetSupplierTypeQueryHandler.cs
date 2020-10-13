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

    public class GetSupplierTypeQueryHandler : IRequestHandler<GetSupplierTypeQuery, SuppliertypeRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IFinanceServerRequest _financeServer;
        public GetSupplierTypeQueryHandler(
            ISupplierRepository supplierRepository,
            IFinanceServerRequest financeServerRequest)
        {
            _financeServer = financeServerRequest;
            _supRepo = supplierRepository;
        }
        public async Task<SuppliertypeRespObj> Handle(GetSupplierTypeQuery request, CancellationToken cancellationToken)
        {
            var item = await _supRepo.GetSupplierTypeAsync(request.SupplierTypeId);
            var Tax = await _supRepo.GetAllTaxSetupAsync();
            var gls = await _financeServer.GetAllSubglAsync();
            var respList = new List<SuppliertypeObj>();
            if (item != null)
            {
                var respItem = new SuppliertypeObj
                {
                    Active = item.Active,
                    CreatedBy = item.CreatedBy,
                    CreatedOn = item.CreatedOn,
                    SupplierTypeId = item.SupplierTypeId,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedOn = item.UpdatedOn,
                    SupplierTypeName = item.SupplierTypeName,
                    CreditGL = item.CreditGL,
                    DebitGL = item.DebitGL,
                    TaxApplicable = item.TaxApplicable.Split(',').Select(int.Parse).ToArray(),
                };
                respList.Add(respItem);
            }
            if (respList.Count() > 0)
            {
                foreach (var litem in respList)
                {
                    litem.TaxApplicableName = Tax.FirstOrDefault(d => litem.TaxApplicable.Contains(d.TaxSetupId))?.TaxName;
                    litem.CreditSubGlCode = gls.SubGls.FirstOrDefault(q => q.subGLId == item.CreditGL)?.subGLCode;
                    litem.DebitSubGlCode = gls.SubGls.FirstOrDefault(q => q.subGLId == item.DebitGL)?.subGLCode;
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
                        FriendlyMessage = respList.Count() > 0 ? null : "Search Complete!! No Record Found"
                    }
                }
            };
        }
    }
}
