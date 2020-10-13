using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.FinanceServer;
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
    public class PurchasesTrendAnalysis : IRequest<PayableDaysTrendAnalysisResp>
    {
        public class PurchasesTrendAnalysisHandler : IRequestHandler<PurchasesTrendAnalysis, PayableDaysTrendAnalysisResp>
        {
            private readonly DataContext _dataContext;
            private readonly IFinanceServerRequest _financeServer;
            public PurchasesTrendAnalysisHandler(DataContext dataContext, IFinanceServerRequest financeServer)
            {
                _financeServer = financeServer;
                _dataContext = dataContext;
            }
            public async Task<PayableDaysTrendAnalysisResp> Handle(PurchasesTrendAnalysis request, CancellationToken cancellationToken)
            {
                var response = new PayableDaysTrendAnalysisResp { Datasets = new List<PayableDays>(), Status = new APIResponseStatus { Message = new APIResponseMessage() } };
                try
                {
                    #region LATER

                    //var payable = (from a in _dataContext.cor_paymentterms
                    //               join b in _dataContext.inv_invoice on a.PaymentTermId equals b.PaymentTermId
                    //               join c in _dataContext.cor_supplier on b.SupplierId equals c.SupplierId
                    //               select
                    //              new SupplierData
                    //              {
                    //                  Date = a.CompletionDate ?? DateTime.UtcNow,
                    //                  SuppliertypeId = c.SupplierTypeId,
                    //                  SuppliierId = b.SupplierId,
                    //                  CompletionDate = a.CompletionDate,
                    //                  InvoiceGenerated = a.InvoiceGenerated
                    //              }).ToList();

                    //var groupedpayables = payable.GroupBy(u => u.SuppliertypeId).Select(grp => grp.ToList()).ToList();


                    //var groupedpurchases = payable.GroupBy(u => u.SuppliertypeId).Select(grp => grp.ToList().Select(f => new SupplierData
                    //{
                    //    SuppliertypeId = f.SuppliertypeId,
                    //    Date = f.Date,
                    //    CompletionDate = f.CompletionDate,
                    //    InvoiceGenerated = f.InvoiceGenerated,
                    //    SuppliierId = f.SuppliierId,
                    //})).ToList();
                    //var pyablesday = new List<FinalDataAnalytics>();
                    //foreach (var gp in groupedpurchases)
                    //{
                    //    var purchs = 0;
                    //    var pybls = 0;
                    //    var sequence1 = gp.Where(q => q.InvoiceGenerated && q.CompletionDate == null).ToList().Select(q => q.Date.TimeOfDay);
                    //    if(sequence1.ToList().Count() > 0)
                    //    {
                    //         pybls = (sequence1.Aggregate((sum, next) => sum + next)).DaysTrendAnalysis;
                    //    }
                    //    var sequence2 = gp.Where(q => q.InvoiceGenerated && q.CompletionDate != null).ToList().Select(q => q.Date.TimeOfDay);
                    //    if (sequence2.ToList().Count() > 0)
                    //    {
                    //         purchs = (sequence2.Aggregate((sum, next) => sum + next)).DaysTrendAnalysis;
                    //    }

                    //    var pbDaysTrendAnalysis = new FinalDataAnalytics
                    //    {
                    //        SuppliertypeName = _dataContext.cor_suppliertype.FirstOrDefault(q => q.SupplierTypeId == gp.FirstOrDefault().SuppliertypeId)?.SupplierTypeName,
                    //        PayableDaysTrendAnalysis = purchs / pybls * 365
                    //    };
                    //    pyablesday.Add(pbDaysTrendAnalysis);
                    //}

                    //List<int> dataValue = new List<int>();
                    //var Labels = new List<string>();
                    //var datas = new List<PayableDaysTrendAnalysis>();

                    //foreach (var w in pyablesday)
                    //{
                    //    Labels.Add(w.SuppliertypeName);
                    //    dataValue.Add(w.PayableDaysTrendAnalysis); 
                    //}
                    //var dataset = new PayableDaysTrendAnalysis
                    //{
                    //    Label = "Payable DaysTrendAnalysis",
                    //    BorderColor = "",
                    //    Data = dataValue.ToArray(),
                    //    Fill = false,
                    //};
                    //datas.Add(dataset);
                    //response.PayableDaysTrendAnalysis = datas;
                    //response.Labels = Labels.ToArray();
                    #endregion
                      
                    var suppliertypes = _dataContext.cor_suppliertype.ToList();

                    var gls = new List<int>(); 
                    var debitGls = new List<int>();
                     
                    debitGls = suppliertypes.Select(r => r.DebitGL).Distinct().ToList(); 
                    gls.AddRange(debitGls);

                    var reportgls = new ReportGls
                    {
                        Trans = gls
                    };

                    var transactions = await _financeServer.GetFinTransactions(reportgls); 

                    if (!transactions.Status.IsSuccessful)
                    {
                        response.Status = transactions.Status;
                        return response;
                    }

                    List<decimal> dataValue = new List<decimal>();
                    var Labels = new List<string>();
                    var datas = new List<PayableDays>();


                    var byYearGroup = transactions.Trans.GroupBy(r => r.TransactionDate.Year).Select(grp => grp.ToList()).ToList();
                    foreach(var group in byYearGroup)
                    {
                        var Year = group.FirstOrDefault().TransactionDate.Year.ToString();
                        var total = group.Where(t => debitGls.Contains(t.SubGL)).ToList().Sum(r => r.DebitAmount);
                        Labels.Add(Year);
                        dataValue.Add(total);
                    } 
 
                    
                    var dataset = new PayableDays
                    {
                        Label = "Purchases Trend Analysis",
                        BorderColor = "",
                        Data = dataValue.ToArray(),
                        Fill = false,
                    };
                    datas.Add(dataset);
                    response.Datasets = datas;
                    response.Labels = Labels.ToArray();

                    // transactions.CreditEntries

                    return await Task.Run(() =>  response);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           
        }

        public class GroupedByYears
        {
            public string Year { get; set; }
            public decimal TotoalSum { get; set; } 
        }
        public class cp
        {
            public int typeid { get; set; }
            public string TypeName { get; set; }
            public decimal Amount { get; set; }
            public int SubGl { get; set; }
            public int transid { get; set; }
        }
    }
   
}
