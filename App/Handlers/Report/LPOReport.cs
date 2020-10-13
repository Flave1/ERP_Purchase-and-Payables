using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.Report;
using Puchase_and_payables.Data;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Report
{
    public class LPOReportQuery : IRequest<LPOReport>
    {
        public LPOReportQuery() { }
        public int LPOId { get; set; }
        public LPOReportQuery(int LPOid)
        {
            LPOId = LPOid;
        }
        public class LPOReportQueryHandler : IRequestHandler<LPOReportQuery, LPOReport>
        {
            private readonly DataContext _dataContext;
            private readonly IIdentityServerRequest _serverRequest;
            public LPOReportQueryHandler(DataContext dataContext, IIdentityServerRequest serverRequest)
            {
                _serverRequest = serverRequest;
                _dataContext = dataContext;
            }
            public async Task<LPOReport> Handle(LPOReportQuery request, CancellationToken cancellationToken)
            {
                var response = new LPOReport
                {
                    Phases = new List<Phases>(),
                    ServiceTerm = new ServiceTerm(),
                    From = new From(),
                    To = new To(),
                    Status = new APIResponseStatus { Message = new APIResponseMessage(), IsSuccessful = true, },
                };
                try
                {
                    var staff = await _serverRequest.GetAllStaffAsync();

                    var lpo = _dataContext.purch_plpo.FirstOrDefault(q => q.PLPOId == request.LPOId);
                    var prn = _dataContext.purch_requisitionnote.FirstOrDefault(q => q.PurchaseReqNoteId == lpo.PurchaseReqNoteId);

                    var serviceterms = _dataContext.cor_serviceterms.Where(q => q.ServiceTermsId == lpo.ServiceTerm).Select(w => new ServiceTerm
                    {
                        Content = w.Content,
                        Header = w.Header
                    }).ToList()??new List<ServiceTerm>();

                    var phases = _dataContext.cor_paymentterms.Where(w => w.LPOId == lpo.PLPOId && w.ProposedBy == (int)Proposer.STAFF).Select(w => new Phases
                    {
                        Amount = w.Amount, 
                        Tax = w.TaxPercent,
                        Phase = w.Phase
                    }).ToList();

                    var companies = await _serverRequest.GetAllCompanyStructureAsync();
                    var to = companies.companyStructures.Where(e => e.ParentCompanyID == 0).Select(r => new To { 
                        Address = r.Address1,
                        Name = r.Name,
                        Number = r.Telephone
                    }).ToList();

                    var from = _dataContext.cor_supplier.Where(q => q.SupplierId == lpo.WinnerSupplierId).Select(s => new From
                    {
                        SupplierId = s.SupplierId,
                        Address = s.Address,
                        Name = s.Name,
                        Number = s.SupplierNumber
                    }).ToList();

                    response.AmountPayable = lpo.AmountPayable;
                    response.GrossAmount = lpo.GrossAmount;
                    response.LPONumber = lpo.LPONumber;
                    response.Quantity = lpo.Quantity;
                    response.RequestDate = lpo.RequestDate;
                    response.TaxAmount = lpo.Tax;
                    response.From = from.FirstOrDefault();
                    response.ServiceTerm = serviceterms.FirstOrDefault();
                    response.Phases = phases;
                    response.Description = lpo.Description;
                    response.Tax = lpo.Tax;
                    response.To = to.FirstOrDefault(); 

                    return response;
                }
                catch (Exception ex)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = ex.Message;
                    return response;
                }  
            }
        }
    }
}
