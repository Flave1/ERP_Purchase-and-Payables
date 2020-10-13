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
using GOSLibraries.Enums;
using Puchase_and_payables.DomainObjects.Supplier;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class GetAllPendingSupplierQuery : IRequest<SupplierRespObj>
    {
        public class GetAllPendingSupplierQueryHandler : IRequestHandler<GetAllPendingSupplierQuery, SupplierRespObj>
        {
            private readonly ISupplierRepository _supRepo;
            private readonly IMapper _mapper;
            public GetAllPendingSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
            {
                _mapper = mapper;
                _supRepo = supplierRepository;
            }
            public async Task<SupplierRespObj> Handle(GetAllPendingSupplierQuery request, CancellationToken cancellationToken)
            {

                var supplierType = await _supRepo.GetAllSupplierTypeAsync();

                var supplierList = await _supRepo.GetAllSupplierAsync();
                var data = _mapper.Map<List<SupplierObj>>(supplierList);
                var respList = new List<SupplierObj>();
                foreach (var sup in data.Where(q => q.ApprovalStatusId == (int)ApprovalStatus.Pending))
                {
                    var supplierBankAccount = _supRepo.GetAllBankAccountdetails(sup.SupplierId);
                    if (supplierBankAccount.Count() < 0 || supplierBankAccount == null)
                    {
                        supplierBankAccount = new List<cor_bankaccountdetail>();
                    }
                    sup.AccountName = supplierBankAccount.FirstOrDefault()?.AccountName;
                    sup.AccountNumber = supplierBankAccount.FirstOrDefault()?.AccountNumber;
                    sup.BVN = supplierBankAccount.FirstOrDefault()?.BVN;
                    sup.StatusName = Convert.ToString((ApprovalStatus)sup.ApprovalStatusId);
                    sup.SupplierTypeName = supplierType.FirstOrDefault(s => s.SupplierTypeId == sup.SupplierTypeId)?.SupplierTypeName ?? "";
                    respList.Add(sup);
                }
                return new SupplierRespObj
                {
                    Suppliers = data,
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = supplierList.Count() == 0 ? "Search Complete!! No Record Found" : null
                        }
                    }
                };
            }
        }
    }
    
}
