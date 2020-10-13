using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.Report;
using Puchase_and_payables.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Report
{
    public class AgingAnalysisTableQuery : IRequest<AgingAnalysisRespObj>
    { 
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; } 
        public class AgingAnalysisTableQueryHandler : IRequestHandler<AgingAnalysisTableQuery, AgingAnalysisRespObj>
        {
            private readonly DataContext _dataContext;
            public AgingAnalysisTableQueryHandler(DataContext dataContext)
            {
                _dataContext = dataContext;
            }
            public async Task<AgingAnalysisRespObj> Handle(AgingAnalysisTableQuery request, CancellationToken cancellationToken)
            {
                var response = new AgingAnalysisRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } }; 
                var aginanalysis = new List<AagingAnalysisTable>();
                try
                {
                    if(request.FromDate != null && request.ToDate != null)
                    {
                        response.AagingAnalysis = (from a in _dataContext.cor_paymentterms
                                                    join b in _dataContext.inv_invoice on a.LPOId equals b.LPOId 
                                                    join c in _dataContext.cor_supplier on b.SupplierId equals c.SupplierId
                                                    where a.EntryDate >= request.FromDate || a.CompletionDate <= request.ToDate
                                                    where a.PaymentStatus == (int)PaymentStatus.Not_Paid || a.PaymentStatus == (int)PaymentStatus.Paid  && a.ProposedBy == (int)Proposer.STAFF
                                                    select new AagingAnalysisTable
                                                   {
                                                       InvoiceNumber = b.InvoiceNumber,
                                                       ItemDescription = b.Description, 
                                                       Date = a.EntryDate, 
                                                       SupplierId = b.SupplierId,
                                                       SupplierName = c.Name,
                                                       SupplierNumber = c.SupplierNumber,
                                                       Day_0_30 =  (DateTime.Now - a.EntryDate).Days >= 0
                                                           &&  (DateTime.Now - a.EntryDate).Days <= 31 ? b.Amount : 0,

                                                       Day_31_60 = (DateTime.Now - a.EntryDate).Days >= 31
                                                           && (DateTime.Now - a.EntryDate).Days <= 60 ? b.Amount : 0,

                                                       Day_61_90 = (DateTime.Now - a.EntryDate).Days >= 61
                                                           && (DateTime.Now - a.EntryDate).Days <= 90 ? b.Amount : 0,

                                                       Day_91_180 = (DateTime.Now - a.EntryDate).Days >= 91
                                                           && (DateTime.Now - a.EntryDate).Days <= 180 ? b.Amount : 0,

                                                       Day_180_Above = (DateTime.Now - a.EntryDate).Days >= 180 ? b.Amount : 0,

                                                       Total_Days = (DateTime.Now - a.EntryDate).Days
                                                   }).ToList();
                        var res = response.AagingAnalysis.GroupBy(w => w.InvoiceNumber).Select(e => e.Last()).ToList();

                        foreach (var result in res)
                        {
                            result.SupplierName = _dataContext.cor_supplier.FirstOrDefault(w => w.SupplierId == result.SupplierId)?.Name;
                            result.SupplierNumber = _dataContext.cor_supplier.FirstOrDefault(w => w.SupplierId == result.SupplierId)?.SupplierNumber;
                        }
                        response.AagingAnalysis = res;
                        return await Task.Run(() => response);
                    }
                    return response;
                    
                }
                catch (Exception ex)
                {
                    response.Status.Message.FriendlyMessage = ex.Message;
                    return response;
                }
            }
        }
    }
}
