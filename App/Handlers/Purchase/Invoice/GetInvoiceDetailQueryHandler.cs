using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.Data;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Repository.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{ 

    public class GetInvoiceDetailQueryHandler : IRequestHandler<GetInvoiceDetailQuery, InvoiceRespObj>
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ISupplierRepository _supRepo;
        private readonly DataContext _dataContext;
        public GetInvoiceDetailQueryHandler(
            IInvoiceService invoiceService,
            ISupplierRepository supplierRepository, 
            DataContext dataContext)
        {
            _supRepo = supplierRepository;
            _invoiceService = invoiceService;
            _dataContext = dataContext;
        }
        public async Task<InvoiceRespObj> Handle(GetInvoiceDetailQuery request, CancellationToken cancellationToken)
        {
            var response = new InvoiceRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            var supplierList = await _supRepo.GetAllSupplierAsync();

            var query = _dataContext.inv_invoice.FromSqlRaw($"SELECT * FROM dbo.inv_invoice WHERE InvoiceId = {request.InvoiceId}").FirstOrDefault();

            //var query = await _invoiceService.GetInvoiceAsync(request.InvoiceId);

            InvoiceObj item = new InvoiceObj();
            List<InvoiceObj> resp = new List<InvoiceObj>();
            if (query != null)
            {
                item = new InvoiceObj
                {
                    LPONumber = query.LPONumber,
                    InvoiceNumber = query.InvoiceNumber,
                    Supplier = supplierList.FirstOrDefault(s => s.SupplierId == query.SupplierId)?.Name,
                    InvoiceId = query.InvoiceId,
                    RequestDate = query.RequestDate,
                    Location = query.Address,
                    AmountPayable = query.AmountPayable,
                    DescriptionOfRequest = query.Description,
                    PaymentOutstanding = query.AmountPayable,
                    ExpectedDeliveryDate = query.DeliveryDate,
                    PaymentTermId = query.PaymentTermId,
                    AmountPaid = 0,
                    Amount = query.Amount,
                    LpoId = query.LPOId,
                    SupplierId = query.SupplierId, 
                    Tax = _dataContext.cor_paymentterms.FirstOrDefault(q => q.PaymentTermId == query.PaymentTermId).TaxPercent,
            };
                var alreadyPaidPhases = _dataContext.cor_paymentterms.Where(a => a.LPOId == item.LpoId && a.ProposedBy == (int)Proposer.STAFF && a.PaymentStatus == (int)PaymentStatus.Paid).ToList();
                
                if (alreadyPaidPhases.Count() > 0)
                {
                    item.AmountPaid = alreadyPaidPhases.Sum(f => f.NetAmount);
                    item.PaymentOutstanding = (item.AmountPayable - alreadyPaidPhases.Sum(f => f.NetAmount));
                }
                item.Phase = _dataContext.cor_paymentterms.FirstOrDefault(q => q.PaymentTermId == item.PaymentTermId).Phase;
                //item.Tax = (lpo.Tax / _dataContext.cor_paymentterms.Count(r => r.LPOId == item.LpoId && r.ProposedBy == (int)Proposer.STAFF));
                item.GrossAmount = item.Amount;
                item.PaymentTerms = _dataContext.cor_paymentterms.Where(r => r.ProposedBy == (int)Proposer.STAFF && r.LPOId == item.LpoId).Select(c => new PaymentTermsObj
                {
                    BidAndTenderId = c.BidAndTenderId,
                    Comment = c.Comment,
                    Completion = c.Completion,
                    Amount = c.Amount,
                    NetAmount = c.NetAmount,
                    Payment = c.Payment,
                    PaymentStatus = c.PaymentStatus,
                    PaymentTermId = c.PaymentTermId,
                    Phase = c.Phase,
                    ProjectStatusDescription = c.ProjectStatusDescription,
                    Status = c.Status,
                    PhaseTax =  c.TaxPercent, //Convert.ToDouble(lpo.Tax / _dataContext.cor_paymentterms.Count(r => r.LPOId == item.LpoId && r.ProposedBy == (int)Proposer.STAFF)),
                    PaymentStatusName = Convert.ToString((PaymentStatus)c.PaymentStatus),
                    ProposedBy = c.ProposedBy,
                    StatusName = Convert.ToString((JobProgressStatus)c.Status)
                }).ToList();
                resp.Add(item);
            }


            response.Invoices = resp;
            response.Status.IsSuccessful = true;
            return response;
        }
    }
}
