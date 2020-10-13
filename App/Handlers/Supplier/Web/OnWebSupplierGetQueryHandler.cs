using AutoMapper;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.RequestResponse.Supplier;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Puchase_and_payables.Contracts.Queries.Supplier.Web;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Repository.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Supplier.Web
{
    public class OnWebSupplierGetQueryHandler : IRequestHandler<OnWebSupplierGetQuery, SupplierRespObj>
    {
        private readonly IOnWebSupplierService _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OnWebSupplierGetQueryHandler(
            IMapper mapper, 
            IOnWebSupplierService supplierRepository, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _repo = supplierRepository;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<SupplierRespObj> Handle(OnWebSupplierGetQuery request, CancellationToken cancellationToken)
        {
            var loggedInUserId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

            var supplierList = await _repo.OnWebSupplierGetAsync(loggedInUser.Email);
            return new SupplierRespObj
            {
                Suppliers = _mapper.Map<List<SupplierObj>>(supplierList),
                Status = new APIResponseStatus
                {
                    IsSuccessful = true,
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = supplierList.Count() == 0 ? "Search Complete!! No Record Found" : null
                    }
                }
            };
        }
    }
}
