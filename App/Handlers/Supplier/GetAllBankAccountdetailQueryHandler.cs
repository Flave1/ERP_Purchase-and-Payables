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

namespace Puchase_and_payables.Handlers
{
   
    public class GetAllBankAccountdetailQueryHandler : IRequestHandler<GetAllBankAccountdetailQuery, SupplierAccountRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IFinanceServerRequest _financeServer;
        public GetAllBankAccountdetailQueryHandler(
            ISupplierRepository supplierRepository,
            IFinanceServerRequest financeServer)
        {
            _financeServer = financeServer;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierAccountRespObj> Handle(GetAllBankAccountdetailQuery request, CancellationToken cancellationToken)
        {
            var list =  _supRepo.GetAllBankAccountdetails(request.SupplierId);
            var banks = await _financeServer.GetAllBanksAsync();
            return new SupplierAccountRespObj
            {
                SupplierAccountBankDetails = list.Select(x => new SupplierBankAccountDetailsObj()
                {
                    UpdatedOn = x.UpdatedOn,
                    Active = x.Active,  
                    CreatedOn = x.CreatedOn,
                    Deleted = x.Deleted,  
                    SupplierId = x.SupplierId,
                    UpdatedBy = x.UpdatedBy,
                    CompanyId = x.CompanyId,
                    BVN = x.BVN,
                    AccountNumber = x.AccountNumber,
                    AccountName = x.AccountName,
                    BankAccountDetailId = x.BankAccountDetailId,
                    CreatedBy = x.CreatedBy,
                    Bank = x.Bank,
                    BankName = banks.bank.FirstOrDefault(q => q.BankGlId == x.Bank)?.BankName
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
