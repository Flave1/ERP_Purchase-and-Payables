using MediatR;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase.Invoice
{
    public class GetBanskQuery : IRequest<BanksRespObj>
    {
        public class GetBanskQueryHandler : IRequestHandler<GetBanskQuery, BanksRespObj>
        {
            private readonly IFinanceServerRequest _financeServer;
            public GetBanskQueryHandler(IFinanceServerRequest financeServer)
            {
                _financeServer = financeServer;
            }
            public async Task<BanksRespObj> Handle(GetBanskQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await _financeServer.GetAllBanksAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }

}
