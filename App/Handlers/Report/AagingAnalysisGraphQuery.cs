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
    public class AgingAnalysisGraphQuery : IRequest<DashboardAgeAnalysisResp>
    {  
        public class AgingAnalysisGraphQueryHandler : IRequestHandler<AgingAnalysisGraphQuery, DashboardAgeAnalysisResp>
        {
            private readonly DataContext _dataContext;
            public AgingAnalysisGraphQueryHandler(DataContext dataContext)
            {
                _dataContext = dataContext;
            }
            public async Task<DashboardAgeAnalysisResp> Handle(AgingAnalysisGraphQuery request, CancellationToken cancellationToken)
            {
               var response = new DashboardAgeAnalysisResp { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } }; 
                var aginanalysis = new List<AagingAnalysisGraph>();
                try
                {
                   var result = (from a in _dataContext.cor_paymentterms
                                               join b in _dataContext.inv_invoice on a.LPOId equals b.LPOId
                                               join c in _dataContext.cor_supplier on b.SupplierId equals c.SupplierId 
                                               where a.PaymentStatus == (int)PaymentStatus.Not_Paid || a.PaymentStatus == (int)PaymentStatus.Paid && a.ProposedBy == (int)Proposer.STAFF
                                               select new AagingAnalysisGraph
                                               {
                                                   InvoiceNumber = b.InvoiceNumber,
                                                   ItemDescription = b.Description,
                                                   Date = a.EntryDate,
                                                   SupplierId = b.SupplierId,
                                                   SupplierName = c.Name,
                                                   SupplierNumber = c.SupplierNumber,
                                                   Day_0_30 = (DateTime.Now - a.EntryDate).Days >= 0
                                                      && (DateTime.Now - a.EntryDate).Days <= 31 ? b.Amount : 0,

                                                   Day_31_60 = (DateTime.Now - a.EntryDate).Days >= 31
                                                      && (DateTime.Now - a.EntryDate).Days <= 60 ? b.Amount : 0,

                                                   Day_61_90 = (DateTime.Now - a.EntryDate).Days >= 61
                                                      && (DateTime.Now - a.EntryDate).Days <= 90 ? b.Amount : 0,

                                                   Day_91_180 = (DateTime.Now - a.EntryDate).Days >= 91
                                                      && (DateTime.Now - a.EntryDate).Days <= 180 ? b.Amount : 0,

                                                   Day_180_Above = (DateTime.Now - a.EntryDate).Days >= 180 ? b.Amount : 0,

                                                   Total_Days = (DateTime.Now - a.EntryDate).Days
                                               }).OrderByDescending(w => w.Date).Take(10).ToList();
                    var res = result.GroupBy(w => w.InvoiceNumber).Select(e => e.Last()).ToList();

                    //foreach (var item in res)
                    //{
                    //    item.SupplierName = _dataContext.cor_supplier.FirstOrDefault(w => w.SupplierId == item.SupplierId)?.Name;
                    //    item.SupplierNumber = _dataContext.cor_supplier.FirstOrDefault(w => w.SupplierId == item.SupplierId)?.SupplierNumber;
                    //}

                    List<decimal> data = new List<decimal>();
                    List<decimal> dataValue = new List<decimal>();
                    foreach (var w in res)
                    {
                        data.Add(w.Day_0_30);
                        data.Add(w.Day_31_60);
                        data.Add(w.Day_61_90);
                        data.Add(w.Day_91_180);
                        data.Add(w.Day_91_180);
                    }
                    foreach(var data2 in data)
                    {
                        if(data2 != 0)
                        {
                            dataValue.Add(data2);
                        }
                    }
                    response.Labels = new string[] { "0-30", "31-60", "61-90", "91-180", "180-Above" };
                   response.Datasets = new List<DashbordAgeAnalysis>
                        {
                            new DashbordAgeAnalysis
                            {
                                Label = "Age Analysis",
                                BorderColor = "#4bc0c0",
                                Data = dataValue.ToArray(),
                                Fill = false,

                            },
                        };
                    return await Task.Run(() => response);
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
