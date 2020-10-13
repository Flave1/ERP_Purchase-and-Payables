using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Queries.Purchases;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.Purchase;
using Puchase_and_payables.DomainObjects.Supplier;
using Puchase_and_payables.Repository.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class GetAllLPOQueryHandler : IRequestHandler<GetAllLPOQuery, LPORespObj>
    {
        private readonly IPurchaseService _repo; 
        public GetAllLPOQueryHandler(IPurchaseService purchaseService)
        { 
            _repo = purchaseService;
        }
      
        public async Task<LPORespObj> Handle(GetAllLPOQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetALLLPOAsync(); 
            return new LPORespObj
            {
                LPOs = result.Where(s => s.JobStatus != 0 && s.WinnerSupplierId > 0 ).OrderByDescending(q => q.LPONumber).Select(d => new LPOObj
                {
                    SupplierAddress = d.Address,
                    ApprovalStatusId = d.ApprovalStatusId,
                    DeliveryDate = d.DeliveryDate,
                    Description = d.Description,
                    LPONumber = d.LPONumber,
                    Name = d.Name,
                    PLPOId = d.PLPOId,
                    SupplierId = d.SupplierIds,
                    Tax = d.Tax,
                    Total = d.Total,
                    AmountPayable = d.AmountPayable,
                    BidAndTenderId = d.BidAndTenderId,
                    GrossAmount = d.GrossAmount,
                    JobStatus = d.JobStatus,
                    JobStatusName = Convert.ToString((JobProgressStatus)d.JobStatus),
                    RequestDate = d.RequestDate,
                    SupplierNumber = d.SupplierNumber, 
                    Location = d.Address,
                    Quantity = d.Quantity,
                    WorkflowToken = d.WorkflowToken,    
                }).ToList(),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = result.Count() > 0 ? null : "Search Complete! No Record found"
                    }
                }
            };
        }
    }
}


