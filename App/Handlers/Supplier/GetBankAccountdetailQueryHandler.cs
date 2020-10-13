using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Response.Supplier;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier
{
   

    public class GetBankAccountdetailQueryHandler : IRequestHandler<GetBankAccountdetailQuery, SupplierAccountRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IFinanceServerRequest _financeServer;
        public GetBankAccountdetailQueryHandler(
            ISupplierRepository supplierRepository,
            IFinanceServerRequest financeServer)
        {
            _supRepo = supplierRepository;
            _financeServer = financeServer;
        }
        public async Task<SupplierAccountRespObj> Handle(GetBankAccountdetailQuery request, CancellationToken cancellationToken)
        {
            var item = await _supRepo.GetBankAccountdetailAsync(request.BankAccountDetailId);
            var banks = await _financeServer.GetAllBanksAsync();
            var itemRespList = new List<SupplierBankAccountDetailsObj>();
            if (item != null)
            {
                var itemResp = new SupplierBankAccountDetailsObj
                {
                    Active = item.Active,
                    CreatedBy = item?.CreatedBy ?? string.Empty,
                    CreatedOn = item?.CreatedOn,
                    Deleted = item.Deleted,
                    SupplierId = item.SupplierId,
                    UpdatedBy = item?.UpdatedBy,
                    UpdatedOn = item.UpdatedOn,
                    BankAccountDetailId = item.BankAccountDetailId,
                    AccountName = item.AccountName,
                    AccountNumber = item.AccountNumber,
                    BVN = item.BVN,
                    CompanyId = item.CompanyId,
                    Bank = item.Bank,
                    BankName = banks.bank.FirstOrDefault(w => w.BankGlId == item.Bank)?.BankName
                };
                itemRespList.Add(itemResp);
            }
            return new SupplierAccountRespObj
            {
                SupplierAccountBankDetails = itemRespList,
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
