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

namespace Puchase_and_payables.Handlers.Report.Main
{

    public class PurchaseAndPayablesReportQueryHandler : IRequestHandler<PurchaseAndPayablesReportQuery, PurchAndPayaReportResp>
    {
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        public PurchaseAndPayablesReportQueryHandler(DataContext dataContext, IIdentityServerRequest serverRequest)
        {
            _serverRequest = serverRequest;
            _dataContext = dataContext;
        }
        public async Task<PurchAndPayaReportResp> Handle(PurchaseAndPayablesReportQuery request, CancellationToken cancellationToken)
        {
            var response = new PurchAndPayaReportResp
            {
                PurchAndsPayables = new List<PurchAndPayaReport>(),
                Staus = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() }
            };
            try
            {
                if (string.IsNullOrEmpty(request.LPONumber))
                {
                    response.Staus.Message.FriendlyMessage = "LPO Number Required to pull report";
                    response.Staus.IsSuccessful = false;
                    return response;
                }

                var user = await _serverRequest.UserDataAsync();
                var comp = await _serverRequest.GetAllCompanyStructureAsync();
                var responseList = new List<PurchAndPayaReport>();
                var result = (from a in _dataContext.purch_plpo 
                              join b in _dataContext.purch_requisitionnote on a.PurchaseReqNoteId equals b.PurchaseReqNoteId
                              join c in _dataContext.inv_invoice on a.PLPOId equals c.LPOId
                              join e in _dataContext.cor_supplier on c.SupplierId equals e.SupplierId 
                              select new PurchAndPayaReport
                              {
                                  LPONumber = a.LPONumber,
                                  Description = a.Description,
                                  RequestDate = a.RequestDate,
                                  SupplierId = e.SupplierId,
                                  SupplierName = e.Name,
                                  ExpectedDeleiveryDate = b.ExpectedDeliveryDate.Value,
                                  DeleiveryLocation = a.Address,
                                  AmountPayable = a.AmountPayable,
                                  AmountPaid = c.AmountPaid,
                                  OutStandingAmount = (c.Amount - c.AmountPaid),
                                  LPOId = a.PLPOId,
                                  DepartmentId = b.DepartmentId, 
                                  SupplierLocation = e.Address,
                                  Department = b.DepartmentId,
                                  RequisitionDate = a.RequestDate
                              }).ToList();

             
                if (result.Count() > 0)
                {
                    result = result.Where(r => r.LPONumber == request.LPONumber).ToList();
                    responseList.AddRange(result);
                    foreach (var item in result)
                    {
                        item.DepartmentName = comp.companyStructures.FirstOrDefault(r => r.CompanyStructureId == item.DepartmentId)?.Name;
                    }

                    if (!string.IsNullOrEmpty(request.ItemDescription))
                    {
                        var descs = result.Where(r => r.Description == request.ItemDescription).ToList();
                        if(descs.Count() > 0)
                        {
                            responseList.AddRange(descs);
                        } 
                    }
                    if (string.IsNullOrEmpty(request.SupplierLocation))
                    {
                        var loca = result.Where(r => r.SupplierLocation == request.SupplierLocation).ToList();
                        if(loca.Count() > 0)
                        {
                            responseList.AddRange(loca);
                        }
                    }
                    if (string.IsNullOrEmpty(request.DeleiveryLocation))
                    {
                        var deLoc = result.Where(r => r.DeleiveryLocation == request.DeleiveryLocation).ToList();
                        if(deLoc.Count() > 0)
                        {
                            responseList.AddRange(deLoc);
                        }
                    }
                    if (request.RequisitionDate != null)
                    {
                        var reqDate = result.Where(r => r.RequisitionDate == request.RequisitionDate).ToList();
                        if(reqDate.Count() > 0)
                        {
                            responseList.AddRange(reqDate);
                        }
                    }
                    if (request.Department > 0)
                    {
                        var dept = result.Where(r => r.Department == request.Department).ToList();
                        if(dept.Count() > 0)
                        {
                            responseList.AddRange(dept);
                        }
                    }
                    if (request.AmountPayable != 0)
                    {
                        var amtPaya = result.Where(r => r.AmountPayable == request.AmountPayable).ToList();
                        if(amtPaya.Count() > 0)
                        {
                            responseList.AddRange(amtPaya);
                        }
                    }
                    if (request.ExpectedDeleiveryDate != null)
                    {
                        var exp = result.Where(r => r.ExpectedDeleiveryDate == request.ExpectedDeleiveryDate).ToList();
                        if(exp.Count() > 0)
                        {
                            responseList.AddRange(exp);
                        }
                    }
                    if (request.AmountPaid != 0)
                    {
                        var paid = result.Where(r => r.AmountPaid == request.AmountPaid).ToList();
                        if(paid.Count() > 0)
                        {
                            responseList.AddRange(paid);
                        }
                    }
                    if (request.OutStandingAmount != 0)
                    {
                        //foreach (var item in result)
                        //{
                        //    item.OutStandingAmount = item.AmountPayable - item.AmountPaid;
                        //}
                        //responseList.AddRange(result.Where(r => r.AmountPaid == request.AmountPaid).ToList());
                    }
                     
                }
                 
               //if(result.Count() > 0)
               // {
               //     //foreach (var item in result)
               //     //{
               //     //    item.SubStrucure = comp.companyStructures.FirstOrDefault(r => r.CompanyStructureId == Convert.ToInt64(user.CompanyId))?.Name;
               //     //    item.Total = item.LPOReports.Select(r => r.GrossAmount).ToList().Sum();
               //     //    item.NumberOfLPO = item.LPOReports.Select(r => r.GrossAmount).ToList().Count();
               //     //    foreach (var sub in item.LPOReports)
               //     //    {
               //     //        sub.DepartmentName = comp.companyStructures.FirstOrDefault(r => r.CompanyStructureId == sub.DepartmentId)?.Name;
               //     //    }
               //     //}
               // }
                response.PurchAndsPayables = responseList;
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
