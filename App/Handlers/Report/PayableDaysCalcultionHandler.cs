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
    public class PayableDaysCalcultion : IRequest<PayableDaysTrendAnalysisResp>
    {
        public class PayableDaysCalcultionHandler : IRequestHandler<PayableDaysCalcultion, PayableDaysTrendAnalysisResp>
        {
            private readonly DataContext _dataContext;
            private readonly IFinanceServerRequest _financeServer;
            public PayableDaysCalcultionHandler(DataContext dataContext, IFinanceServerRequest financeServer)
            {
                _financeServer = financeServer;
                _dataContext = dataContext;
            }
            public class DayCalc
            {
                public int typeid { get; set; }
                public string typename { get; set; }
                public decimal CreditAmount { get; set; }
                public decimal DebitAmount { get; set; }
                public int SubGl { get; set; }
                public int tranId { get; set; }
                public decimal RunningBal {get;set;}
            }
            public async Task<PayableDaysTrendAnalysisResp> Handle(PayableDaysCalcultion request, CancellationToken cancellationToken)
            {
                var response = new PayableDaysTrendAnalysisResp { Datasets = new List<PayableDays>(), Status = new APIResponseStatus { Message = new APIResponseMessage() } };
                try
                { 

                    var suppliertypes = _dataContext.cor_suppliertype.ToList();

                    var gls = new List<int>();
                    var creditGls = new List<int>();
                    var debitGls = new List<int>();

                    creditGls = suppliertypes.Select(r => r.CreditGL).Distinct().ToList();
                    debitGls = suppliertypes.Select(r => r.DebitGL).Distinct().ToList();
                    gls.AddRange(creditGls);
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


                    var byTyppeGroup = (from a in transactions.Trans
                                        from b in suppliertypes
                                        orderby a.TransactionDate
                                        where gls.Contains(a.SubGL) select new DayCalc
                                        {
                                          typeid = b.SupplierTypeId,
                                          DebitAmount = a.DebitAmount,
                                          typename = b.SupplierTypeName,
                                          CreditAmount = a.CreditAmount,
                                          SubGl = a.SubGL,
                                          tranId = a.TransactionId,
                                          RunningBal = a.RunningBal??0
                                        }).ToList().GroupBy(r => r.typeid).Select(grp => grp.ToList()).ToList();
                    foreach (var group in byTyppeGroup)
                    {
                        var type = group.FirstOrDefault().typename;
                        var total = group.Where(t => t.typeid == group.FirstOrDefault().typeid).ToList().Sum(r => r.DebitAmount) / group.Where(t => t.typeid == group.FirstOrDefault().typeid).ToList().OrderBy(t => t.tranId).FirstOrDefault().RunningBal * 365;
                        Labels.Add(type);
                        dataValue.Add(total);
                    }


                    var dataset = new PayableDays
                    {
                        Label = "Purchases Analysis",
                        BorderColor = "",
                        Data = dataValue.ToArray(),
                        Fill = false,
                    };
                    datas.Add(dataset);
                    response.Datasets = datas;
                    response.Labels = Labels.ToArray();

                    // transactions.CreditEntries

                    return await Task.Run(() => response);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }

}
