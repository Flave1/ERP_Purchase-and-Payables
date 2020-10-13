using AutoMapper;
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
    public class GetAllSupplierDocumentQueryHandler : IRequestHandler<GetAllSupplierDocumentQuery, SupplierDocumentRespObj>
    {
        private readonly ISupplierRepository _supRepo;
        private readonly IMapper _mapper;
        private readonly IIdentityServerRequest _serverRequest;
        public GetAllSupplierDocumentQueryHandler(
            ISupplierRepository supplierRepository, 
            IMapper mapper,
            IIdentityServerRequest serverRequest)
        {
            _mapper = mapper;
            _serverRequest = serverRequest;
            _supRepo = supplierRepository;
        }
        public async Task<SupplierDocumentRespObj> Handle(GetAllSupplierDocumentQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _supRepo.GetAllSupplierDocumentAsync(request.SupplierId); 
            var res = new List<SupplierDocumentObj>();
            if(supplier.Count() > 0)
            {
                res = _mapper.Map<List<SupplierDocumentObj>>(supplier);
                var docs = await _serverRequest.GetAllDocumentsAsync();
                foreach(var item in res)
                { 
                    item.Name = docs.commonLookups.FirstOrDefault(w => w.LookupId == item.DocumentId)?.LookupName;
                }
            }
            return new SupplierDocumentRespObj
            {
                SupplierDocument = res,
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
