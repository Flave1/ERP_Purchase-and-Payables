using AutoMapper;
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
    

    public class GetAllTaxSetupQueryHandler : IRequestHandler<GetAllTaxSetupQuery, TaxsetupRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IFinanceServerRequest _financeServer;
        public GetAllTaxSetupQueryHandler(
            ISupplierRepository supplierRepository, IFinanceServerRequest financeServer)
        {
            _financeServer = financeServer;
            _supRepo = supplierRepository;
        }
        public async Task<TaxsetupRespObj> Handle(GetAllTaxSetupQuery request, CancellationToken cancellationToken)
        {
            var list = await _supRepo.GetAllTaxSetupAsync();
            var gls = await _financeServer.GetAllSubglAsync();

            return new TaxsetupRespObj
            {
                TasxSetups = list.Select(x => new TaxsetupObj()
                {
                    UpdatedOn = x.UpdatedOn,
                    Active = x.Active, 
                    CreatedBy = x.CreatedBy ?? string.Empty,
                    CreatedOn = x.CreatedOn,
                    Deleted = x.Deleted, 
                    TaxSetupId = x.TaxSetupId,
                    UpdatedBy = x.UpdatedBy,
                    CompanyId = x.CompanyId,
                    Percentage = x.Percentage,
                    SubGL = x.SubGL,
                    SubGlName = gls.SubGls.FirstOrDefault(e => e.subGLId == x.SubGL)?.subGLCode,
                    TaxName = x.TaxName,
                    Type = x.Type
                }).ToList(),
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
