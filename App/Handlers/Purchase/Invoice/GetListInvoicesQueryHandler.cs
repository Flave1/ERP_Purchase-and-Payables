using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Repository.Purchase;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 
    public class GetListInvoicesQueryHandler : IRequestHandler<GetListInvoicesQuery, InvoiceRespObj>
    { 
        private readonly ISupplierRepository _supRepo;
        private readonly IInvoiceService _invoiceService;
        public GetListInvoicesQueryHandler( 
            ISupplierRepository supplierRepository,
            IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService; 
            _supRepo = supplierRepository;
        }
        public async Task<InvoiceRespObj> Handle(GetListInvoicesQuery request, CancellationToken cancellationToken)
        { 
            var supplierList = await _supRepo.GetAllSupplierAsync();
            var invoiceList = await _invoiceService.GetAllInvoiceAsync();

            return new InvoiceRespObj
            {
                Invoices = invoiceList.Select(d => new InvoiceObj
                { 
                    InvoiceId = d.InvoiceId,
                    LPONumber = d.LPONumber,  
                    AmountPayable = d.AmountPayable,  
                    DescriptionOfRequest = d.Description,
                    ExpectedDeliveryDate = d.DeliveryDate,
                    Location = d.Address,
                    RequestDate = d.RequestDate,
                    InvoiceNumber = d.InvoiceNumber,  
                    Amount = d.Amount,
                    AmountPaid = d.AmountPaid,
                    Supplier = supplierList.FirstOrDefault(s => s.SupplierId ==  d.SupplierId)?.Name,    
                    PaymentTermId = d.PaymentTermId
                }).ToList(),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = invoiceList.Count() > 0 ? null : "Search Complete! No Record found"
                    }
                }
            };
        }
    }
}
