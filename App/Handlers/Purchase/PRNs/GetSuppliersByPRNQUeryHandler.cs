using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Repository.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    

    public class GetSuppliersByPRNQUeryHandler : IRequestHandler<GetSuppliersByPRNQUery, SupplierRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        private readonly IPurchaseService _purchaseRepo;
        public GetSuppliersByPRNQUeryHandler(
            ISupplierRepository supplierRepository, 
            IMapper mapper,
            IPurchaseService purchaseService)
        {
            _mapper = mapper;
            _purchaseRepo = purchaseService;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierRespObj> Handle(GetSuppliersByPRNQUery request, CancellationToken cancellationToken)
        {

            var supplierType = await _supRepo.GetAllSupplierTypeAsync();
            var prnDetails = await _purchaseRepo.GetPrnDetailsByPrnId(request.PRNId);

            var suppliersIds = prnDetails.SelectMany(s => s.SuggestedSupplierId.Split(',').Select(int.Parse)).ToList();
            var supplierList = await _supRepo.GetListOfApprovedSuppliersAsync();
            var data = _mapper.Map<List<SupplierObj>>(supplierList.Where(s => suppliersIds.Contains(s.SupplierId)));
            var respList = new List<SupplierObj>();
            foreach (var sup in data)
            {
                var enumName = (ApprovalStatus)sup.ApprovalStatusId;
                sup.StatusName = enumName.ToString();
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
