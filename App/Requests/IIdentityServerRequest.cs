using GOSLibraries.GOS_Financial_Identity;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Puchase_and_payables.Requests
{
    public interface IIdentityServerRequest
    {
        Task<AuthenticationResult> IdentityServerLoginAsync(string userName, string password);
        Task<UserDataResponseObj> UserDataAsync();
        Task<HttpResponseMessage> StaffApprovalRequestAsync(IndentityServerApprovalCommand request);
        Task<List<StaffObj>> GetAllStaffAsync();
        Task<HttpResponseMessage> GotForApprovalAsync(GoForApprovalRequest  request);

        Task<HttpResponseMessage> GetAnApproverItemsFromIdentityServer();

        Task<CompanyStructureRespObj> GetAllCompanyStructureAsync();

        Task<CommonLookupRespObj> GetAllCountryAsync();

        Task<SendEmailRespObj> SendMessageAsync(EmailMessageObj email);
        Task<CommonLookupRespObj> GetAllDocumentsAsync();
        Task<CommonLookupRespObj> GetSingleCurrencyAsync(int currency);
        Task<SendEmailRespObj> SendSpecificMessageAsync(EmailMessageObj email);
        Task<ActivityRespObj> GetAllActivityAsync();
        Task<UserRoleRespObj> GetUserRolesAsync();
    }
}
