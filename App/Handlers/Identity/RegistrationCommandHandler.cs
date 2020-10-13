using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using GOSLibraries.URI;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Supplier;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth; 
using Puchase_and_payables.Helper.Extensions;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Identity
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly DataContext _dataContext;
        private readonly IBaseURIs _uRIs; 
        private readonly IIdentityServerRequest _serverRequest; 
        public RegistrationCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILoggerService loggerService, 
            DataContext dataContext, 
            IBaseURIs uRIs, 
            IIdentityServerRequest serverRequest)
        {
            _userManager = userManager; 
            _logger = loggerService;  
            _uRIs = uRIs;
            _dataContext = dataContext;
            _serverRequest = serverRequest;
        }
        public async Task<AuthResponse> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {
            var response = new AuthResponse { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            try
            {
                var user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.Email,
                    PhoneNumber = request.MobileNumber,
                    FullName = request.FulName
                };

                var createdUser = await _userManager.CreateAsync(user, request.Password);

                if (!createdUser.Succeeded)
                {
                    response.Status.Message.FriendlyMessage = createdUser.Errors.Select(x => x.Description).FirstOrDefault();
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.Status.Message.FriendlyMessage = "Confirmation email has just been sent to your email";
                //var loginResponse = await _identityService.LoginAsync(user);

                await SendMailToNewSuppliersAsync(user); 
                return response;
            }
            catch (Exception ex)
            {
                return response;
            }
        }

        private async Task SendMailToNewSuppliersAsync(ApplicationUser user)
        {
            
            var em = new EmailMessageObj { FromAddresses = new List<EmailAddressObj>(), ToAddresses = new List<EmailAddressObj>() };
            var ema = new EmailAddressObj
            {
                Address = user.Email,
                Name = user.UserName
            };
            var path2 = $"{_uRIs.SelfClient}/#/auth/login";
            em.ToAddresses.Add(ema);
            em.Subject = "Registration Successful";
            em.Content = $"<p style='float:left'> Dear {user.FullName} <br> " +
                $"Congratulations, your account has been successfully created" +
                $"<br>Please click <a href='{path2}'> here </a>  to access your profile and to complete registration</p>";

            em.SendIt = true;
            em.SaveIt = false;
            em.Template = (int)EmailTemplate.LoginDetails;
           
            await _serverRequest.SendMessageAsync(em);
        }
    }
}
    

