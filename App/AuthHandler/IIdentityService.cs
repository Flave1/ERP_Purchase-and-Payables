using GOSLibraries.GOS_Financial_Identity;
using Microsoft.AspNetCore.Http;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.DomainObjects.Auth;
using System;
using System.Threading.Tasks;

namespace Puchase_and_payables.AuthHandler
{
    public interface IIdentityService
    {
        //Task<AuthenticationResult> RegisterAsync(CustomUserRegistrationReqObj userRegistration);
        Task<AuthenticationResult> ChangePasswsord(ChangePassword pass);
        Task<AuthenticationResult> LoginAsync(ApplicationUser user);
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string token);
        Task<bool> CheckUserAsync(string email);
        Task<UserDataResponseObj> FetchLoggedInUserDetailsAsync(string userId);
        Task<ConfirnmationResponse> ConfirmEmailAsync(ConfirnmationRequest request);
        Task<ConfirnmationResponse> VerifyAsync(string email, string code);

    }
}
