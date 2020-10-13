using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.Report;
using Puchase_and_payables.Data;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Report
{
    public class DashboardCountQuery : IRequest<DashboardCountResp>
    {
        public class DashboardCountQueryHandler : IRequestHandler<DashboardCountQuery, DashboardCountResp>
        { 
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IPurchaseService _purchase; 
            private readonly ISupplierRepository _supplier;

            public DashboardCountQueryHandler(
                IPurchaseService purchase, 
                IIdentityServerRequest serverRequest,
                ISupplierRepository supplierRepository)
            {
                _serverRequest = serverRequest;
                _purchase = purchase; 
                _supplier = supplierRepository;
            }
            public async Task<DashboardCountResp> Handle(DashboardCountQuery request, CancellationToken cancellationToken)
            {
                var response = new DashboardCountResp { DashboardCount = new DashboardCount(), Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                try
                {
                    var result = await _serverRequest.GetAnApproverItemsFromIdentityServer();
                    var data = await result.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<WorkflowTaskRespObj>(data);
                    var tokens = res.workflowTasks.Select(s => s.WorkflowToken).ToList();
                    var targetIds = res.workflowTasks.Select(x => x.TargetId).ToList();

                    response.DashboardCount.SupplierCount =  _supplier.GetSupplierDataAwaitingApprovalAsync(targetIds, tokens).Result.Count();

                    response.DashboardCount.PRNCount =   _purchase.GetPRNAwaitingApprovalAsync(targetIds, tokens).Result.Count();

                    response.DashboardCount.BIDCount = _purchase.GetBidAndTenderAwaitingApprovalAsync(targetIds, tokens).Result.Count();

                    response.DashboardCount.LPOCount = _purchase.GetLPOAwaitingApprovalAsync(targetIds, tokens).Result.Count();

                    response.DashboardCount.PaymentsCount =  _purchase.GetRequestedPaymentsAwaitingApprovalAsync(targetIds, tokens).Result.Count(); 

                    response.DashboardCount.PayablesCount = 0;//_dataContext.purch_requisitionnote.Count(r => tokens.Contains(r.WorkflowToken) && targetIds.Contains(r.PurchaseReqNoteId));
                    return response;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
