using AutoMapper;
using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class GetSupplierDocumentQueryHandler : IRequestHandler<GetSupplierDocumentQuery, SupplierDocumentRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        private readonly IIdentityServerRequest _serverRequest;
        public GetSupplierDocumentQueryHandler(
            ISupplierRepository supplierRepository, 
            IMapper mapper, IIdentityServerRequest serverRequest)
        {
            _serverRequest = serverRequest;
            _mapper = mapper;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierDocumentRespObj> Handle(GetSupplierDocumentQuery request, CancellationToken cancellationToken)
        { 
            var supplier = await _supRepo.GetSupplierDocumentAsync(request.SupplierDocumentId);
            var docs = await _serverRequest.GetAllDocumentsAsync();
            var itemList = new List<cor_supplierdocument>();
            itemList.Add(supplier);
            var resp = _mapper.Map<List<SupplierDocumentObj>>(itemList);
            foreach (var item in resp)
            {
                item.Name = docs.commonLookups.FirstOrDefault(a => a.LookupId == item.DocumentId)?.LookupName;
            }
            return new SupplierDocumentRespObj
            {
                SupplierDocument = resp,
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = supplier == null ? "Search Complete!! No Record Found" : null
                    }
                }
            };
        }
    }
}
