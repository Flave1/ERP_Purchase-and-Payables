using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Repository.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class GetPaymenttermsByIdsQuery : IRequest<ProposalPaymentRespObj>
    {
        public GetPaymenttermsByIdsQuery() { }
        public string GetPaymenttermsByIds { get; set; }
        public GetPaymenttermsByIdsQuery(string getPaymenttermsByIds)
        {
            GetPaymenttermsByIds = getPaymenttermsByIds;
        }
        public class GetPaymenttermsByIdsHandlerQueryHandler : IRequestHandler<GetPaymenttermsByIdsQuery, ProposalPaymentRespObj>
        {
            private readonly IPurchaseService _repo;
            public GetPaymenttermsByIdsHandlerQueryHandler(IPurchaseService purchaseService)
            {
                _repo = purchaseService;
            }
            public async Task<ProposalPaymentRespObj> Handle(GetPaymenttermsByIdsQuery request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.GetPaymenttermsByIds))
                {
                    return new ProposalPaymentRespObj
                    {
                        ProposalPayment = new List<ProposalPayment>(),
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "No payment term selected",
                            }
                        }
                    };
                }
                var res = await _repo.GetPaymenttermsByIdsAsync(request.GetPaymenttermsByIds);
                if(res.Count() > 0)
                {
                    return new ProposalPaymentRespObj
                    {
                        ProposalPayment = res.Select(d => new ProposalPayment
                        {
                            PaymentTermId = d.PaymentTermId,
                            Completion = d.Completion, 
                            NetAmount = d.NetAmount,
                            
                            Payment = d.Payment,
                            PaymentStatus = d.PaymentStatus,
                            PaymentStatusName = Convert.ToString(d.PaymentStatus),
                            Phase = d.Phase,
                            PhaseName = Convert.ToString((PaymentTermsPhase)d.Phase),
                            ProjectStatusDescription = d.ProjectStatusDescription,
                            Status = d.Status,
                            StatusName = Convert.ToString((JobProgressStatus)d.Status),
                        }).ToList(),
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = true,
                            Message = new APIResponseMessage()
                        }
                    };
                }
                return new ProposalPaymentRespObj
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Search Complete! No Record found"
                        }
                    }
                };
            }
        } 
    }
}
