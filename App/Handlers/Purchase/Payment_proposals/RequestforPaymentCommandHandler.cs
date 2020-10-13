using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.URI;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.DomainObjects.Bid_and_Tender;
using Puchase_and_payables.DomainObjects.Invoice;
using Puchase_and_payables.DomainObjects.Purchase;
using Puchase_and_payables.Repository.Invoice;
using Puchase_and_payables.Repository.Purchase;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Purchase
{
    public class RequestforPaymentCommand : IRequest<PaymentTermsRegRespObj>
    {
        public int PaymentTermId { get; set; }
        public class RequestforPaymentCommandHandler : IRequestHandler<RequestforPaymentCommand, PaymentTermsRegRespObj>
        {
            private readonly IPurchaseService _repo;
            private readonly DataContext _dataContext;
            private readonly IIdentityServerRequest _serverRequest;
            private readonly IBaseURIs _uRIs;
            private readonly IHttpContextAccessor _accessor;
            private readonly UserManager<ApplicationUser> _userManager;
            public RequestforPaymentCommandHandler(
                IPurchaseService purchaseService,
                DataContext dataContext,
                IHttpContextAccessor accessor,
                IBaseURIs uRIs,
                IIdentityServerRequest serverRequest,
                UserManager<ApplicationUser> userManager)
            { 
                _repo = purchaseService;
                _dataContext = dataContext;
                _accessor = accessor;
                _uRIs = uRIs;
                _userManager = userManager;
                _serverRequest = serverRequest;  
            }

            
            public async Task<PaymentTermsRegRespObj> Handle(RequestforPaymentCommand request, CancellationToken cancellationToken)
            {
                var response = new PaymentTermsRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
                try
                {
                    var user = await _serverRequest.UserDataAsync();
                    if (user == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable To Process This User";
                        return response;
                    }

                    var paymentProposal = await _repo.GetSinglePaymenttermAsync(request.PaymentTermId);
                    if(paymentProposal.PaymentStatus == (int)PaymentStatus.Paid)
                    {
                        response.Status.Message.FriendlyMessage = "Payment already made for this phase";
                        return response;
                    }
                    var thisphaseLpo = await _repo.GetLPOsAsync(paymentProposal.LPOId);
                    if(thisphaseLpo == null)
                    {
                        response.Status.Message.FriendlyMessage = "Unable to Identify this Phase LPO";
                        return response;
                    }

                    if (paymentProposal.InvoiceGenerated)
                    {
                        response.Status.Message.FriendlyMessage = "Invoice Already generated for this phase";
                        return response;
                    }

                    await SendEmailToOfficerForPaymentAsync(paymentProposal, thisphaseLpo);

                    response.Status.IsSuccessful = true;
                    response.Status.Message.FriendlyMessage = "Payment Request Successfully sent";
                    return response;
                }
                catch (Exception ex)
                {
                    #region Log error to file   
                    response.Status.IsSuccessful = false; 
                    response.Status.Message.FriendlyMessage = "Error occured!! Unable to process item";
                    response.Status.Message.TechnicalMessage = $"Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}";
                    return response; 
                    #endregion
                }
            }
            public async Task SendEmailToOfficerForPaymentAsync(cor_paymentterms phase, purch_plpo lpo)
            {
                var userid = _accessor.HttpContext.User?.FindFirst(q => q.Type == "userId")?.Value;
                var user = await _userManager.FindByIdAsync(userid);
                if (user != null)
                {
                   
                    EmailMessageObj em = new EmailMessageObj { ToAddresses = new List<EmailAddressObj>(), FromAddresses = new List<EmailAddressObj>() };

                    var supplier = _dataContext.cor_supplier.FirstOrDefault(q => q.Email == user.Email);
                    var path = $"#/purchases-and-supplier/lpo?id={phase.LPOId}"; 
                  
                    var path2 = $"{_uRIs.MainClient}/{path}";

                     
                    em.Subject = $"Payment Request";
                    em.Content = $"Supplier with supplier number {supplier.SupplierNumber} <br> is requesting for payment for " +
                        $"the supply of {lpo.Description} <br> on  Phase '{phase.Phase}'" +
                        $" with project status of '{ Convert.ToString((JobProgressStatus)phase.Status)} '"+
                        $"<br> Please click <a href='{path2}'> here </a> to see details of Payment";

                    var frm = new EmailAddressObj
                    {
                        Address = supplier.Email,
                        Name = supplier.Name,
                    };
                    em.FromAddresses.Add(frm);
                    em.ActivitIds = new List<int>();
                    em.ActivitIds.Add(14);
                    em.SendIt = true;
                    em.SaveIt = false;
                    em.Template = (int)EmailTemplate.Advert;

                    await _serverRequest.SendSpecificMessageAsync(em);
                }
              
            }
        } 
    }
}