using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Queries;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class GetAllCountriesQuery : IRequest<CommonLookupRespObj>
    {
        public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, CommonLookupRespObj>
        {
            private readonly IIdentityServerRequest _serverRequest;
            public GetAllCountriesQueryHandler(IIdentityServerRequest serverRequest)
            {
                _serverRequest = serverRequest;
            }
            public async Task<CommonLookupRespObj> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
            {
                return  await _serverRequest.GetAllCountryAsync();
            }
        }
    }

    public class GetAllDocumenttypesQuery : IRequest<CommonLookupRespObj>
    {
        public class GetAllDocumenttypesQueryHandler : IRequestHandler<GetAllDocumenttypesQuery, CommonLookupRespObj>
        {
            private readonly IIdentityServerRequest _serverRequest;
            public GetAllDocumenttypesQueryHandler(IIdentityServerRequest serverRequest)
            {
                _serverRequest = serverRequest;
            }
            public async Task<CommonLookupRespObj> Handle(GetAllDocumenttypesQuery request, CancellationToken cancellationToken)
            {
                return await _serverRequest.GetAllDocumentsAsync();
            }
        }
    }
}
