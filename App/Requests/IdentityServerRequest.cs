using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity; 
using Microsoft.AspNetCore.Http; 
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Puchase_and_payables.Contracts.Commands.Supplier.Approval;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.Response.ApprovalRes;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using Puchase_and_payables.Contracts.Response.IdentityServer.QuickType;
using Puchase_and_payables.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Puchase_and_payables.Requests
{
    public class IdentityServerRequest : IIdentityServerRequest
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private const int MaxRetries = 1;
        private readonly IHttpClientFactory _httpClientFactory;
        private StaffApprovalRegRespObj response = new StaffApprovalRegRespObj();
        private HttpResponseMessage result = new HttpResponseMessage();
        private readonly IHttpContextAccessor _accessor;
        private static HttpClient Client;
        private AuthenticationResult _authResponse = null;
        private readonly ILoggerService _logger;


        public IdentityServerRequest(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor httpContextAccessor,
            ILoggerService loggerService)
        { 
            _accessor = httpContextAccessor;
            _logger = loggerService;
            _httpClientFactory = httpClientFactory;
            _retryPolicy = Policy.Handle<Exception>()
              .WaitAndRetryAsync(MaxRetries, times =>
              TimeSpan.FromMilliseconds(times * 100));        }

        public async Task<AuthenticationResult> IdentityServerLoginAsync(string userName, string password)
        {
            try
            {

                var loginRquest = new UserLoginReqObj
                {
                    UserName = userName,
                    Password = password,
                };
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");

                var jsonContent = JsonConvert.SerializeObject(loginRquest);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await gosGatewayClient.PostAsync(ApiRoutes.Identity.IDENTITYSERVERLOGIN.Trim(), byteContent);

                var accountInfo = await result.Content.ReadAsStringAsync();
                _authResponse = JsonConvert.DeserializeObject<AuthenticationResult>(accountInfo);

                if (_authResponse == null)
                {

                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage { FriendlyMessage = "System Error!! Please contact Administrator" }
                        }
                    };
                }

                if (_authResponse.Token != null)
                {

                    return new AuthenticationResult
                    {
                        Token = _authResponse.Token,
                        RefreshToken = _authResponse.RefreshToken
                    };
                }

                return new AuthenticationResult
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = _authResponse.Status.IsSuccessful,
                        Message = new APIResponseMessage
                        {
                            TechnicalMessage = _authResponse.Status?.Message?.TechnicalMessage,
                            FriendlyMessage = _authResponse.Status?.Message?.FriendlyMessage
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");

                return new AuthenticationResult
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please tyr again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        } 
        public async Task<UserDataResponseObj> UserDataAsync()
        {
            try
            {
                var currentUserId = _accessor.HttpContext.User?.FindFirst(x => x.Type == "userId")?.Value ?? string.Empty;
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorization))
                {
                    return new UserDataResponseObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Error Occurred ! Please Contact Systems Administrator"
                            }
                        }
                    };
                }
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);
                var result = await gosGatewayClient.GetAsync(ApiRoutes.Identity.IDENTITY_SERVER_FETCH_USERDETAILS);
                if (!result.IsSuccessStatusCode)
                {
                    var accountInfo1 = await result.Content.ReadAsStringAsync();
                    var dgf = JsonConvert.DeserializeObject<UserDataResponseObj>(accountInfo1);

                    return new UserDataResponseObj
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage { FriendlyMessage = "Some thing went Wrong!", TechnicalMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}" }
                        }
                    };
                }
                var accountInfo = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDataResponseObj>(accountInfo);
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");

                return new UserDataResponseObj
                {
                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : LoginAsync{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }
        public async Task<HttpResponseMessage> StaffApprovalRequestAsync(IndentityServerApprovalCommand request)
        {
            var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
            string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
            gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(request); 
                     
                    var data = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    result = await gosGatewayClient.PostAsync(ApiRoutes.IdentitySeverWorkflow.STAFF_APPROVAL_REQUEST, data);

                    if (!result.IsSuccessStatusCode)
                    {
                        var accountInfo1 = await result.Content.ReadAsStringAsync();
                        var dgf = JsonConvert.DeserializeObject<UserDataResponseObj>(accountInfo1);
                        new StaffApprovalRegRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                Message = new APIResponseMessage { FriendlyMessage = result.ReasonPhrase }
                            }
                        }; 
                    }
                    return result;
                }
                catch (Exception ex) { throw ex; }
            });
        } 

        public async Task<List<StaffObj>> GetAllStaffAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY"); 

                var response = new StaffRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
                 

                var result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverWorkflow.GET_ALL_STAFF);
                
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<StaffRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                }

                if (!response.Status.IsSuccessful)
                {
                    return new List<StaffObj>();
                } 
                return response.staff;

            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new List<StaffObj>();
                #endregion
            }
        }


        
        public async Task<HttpResponseMessage> GotForApprovalAsync(GoForApprovalRequest request)
        {
            var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
            string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
            gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);


            var jsonContent = JsonConvert.SerializeObject(request);
            var buffer = Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    result = await gosGatewayClient.PostAsync(ApiRoutes.IdentitySeverWorkflow.GO_FOR_APPROVAL, byteContent);
                    var resultString = await result.Content.ReadAsStringAsync();
                    var resultObj = JsonConvert.DeserializeObject<GoForApprovalRespObj>(resultString);
                    if (!result.IsSuccessStatusCode)
                    {
                        new StaffApprovalRegRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                Message = resultObj.Status.Message
                            }
                        };
                    }
                    return result;
                }
                catch (Exception ex) { throw ex; }
            });
        }




        public async Task<HttpResponseMessage> GetAnApproverItemsFromIdentityServer()
        {

            var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
            string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
            gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverWorkflow.GET_ALL_STAFF_AWAITING_APPROVALS);
                    if (!result.IsSuccessStatusCode)
                    {
                        new StaffApprovalRegRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                Message = new APIResponseMessage { FriendlyMessage = result.ReasonPhrase }
                            }
                        };
                    }
                    return result;
                }
                catch (Exception ex) { throw ex; }
            });
        }
          
        public async Task<CompanyStructureRespObj> GetAllCompanyStructureAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");  

                var response = new CompanyStructureRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                CompanyStructureRespObj resultObj = new CompanyStructureRespObj();

                var result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverRequests.COMPANY);
                var resultString = await result.Content.ReadAsStringAsync();
                resultObj = JsonConvert.DeserializeObject<CompanyStructureRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                }

                if (!resultObj.Status.IsSuccessful)
                {
                    response.Status = resultObj.Status;
                    return response;
                }
                response.companyStructures = resultObj.companyStructures;
                return response;

            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        } 

        public async Task<SendEmailRespObj> SendMessageAsync(EmailMessageObj email)
        {
            var response = new SendEmailRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
            try
            { 
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");

                var jsonContent = JsonConvert.SerializeObject(email);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");  
                  
                var result = await gosGatewayClient.PostAsync(ApiRoutes.IdentitySeverRequests.SEND_EMAIL, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<SendEmailRespObj>(resultString); 
                return response; 
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }
        public async Task<SendEmailRespObj> SendSpecificMessageAsync(EmailMessageObj email)
        {
            var response = new SendEmailRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");

                var jsonContent = JsonConvert.SerializeObject(email);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await gosGatewayClient.PostAsync(ApiRoutes.IdentitySeverRequests.SEND_EMAIL_TO_SPECIFIC_OFFICERS, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<SendEmailRespObj>(resultString);
                return response;
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }

        public async Task<CommonLookupRespObj> GetAllCountryAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new CommonLookupRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                CommonLookupRespObj resultObj = new CommonLookupRespObj();

                var result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverRequests.COUNTRY);
                var resultString = await result.Content.ReadAsStringAsync();
                resultObj = JsonConvert.DeserializeObject<CommonLookupRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                }

                if (!resultObj.Status.IsSuccessful)
                {
                    response.Status = resultObj.Status;
                    return response;
                }
                response.commonLookups = resultObj.commonLookups;
                return response;

            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }

        public async Task<CommonLookupRespObj> GetAllDocumentsAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new CommonLookupRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                CommonLookupRespObj resultObj = new CommonLookupRespObj();

                var result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverRequests.DOCUMENT_TYPE);
                var resultString = await result.Content.ReadAsStringAsync();
                resultObj = JsonConvert.DeserializeObject<CommonLookupRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                }

                if (!resultObj.Status.IsSuccessful)
                {
                    response.Status = resultObj.Status;
                    return response;
                }
                response.commonLookups = resultObj.commonLookups;
                return response;

            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }

        public async Task<CommonLookupRespObj> GetSingleCurrencyAsync(int currency)
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new CommonLookupRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                CommonLookupRespObj resultObj = new CommonLookupRespObj();

                var result = await gosGatewayClient.GetAsync($"{ApiRoutes.IdentitySeverRequests.SINGLE_CURRENCY}?CurrencyId={currency}");
                var resultString = await result.Content.ReadAsStringAsync();
                resultObj = JsonConvert.DeserializeObject<CommonLookupRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                }

                if (!resultObj.Status.IsSuccessful)
                {
                    response.Status = resultObj.Status;
                    return response;
                }
                response.commonLookups = resultObj.commonLookups;
                return response; 
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }

        public async Task<ActivityRespObj> GetAllActivityAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new ActivityRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
                  
                var result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverRequests.ACTIVITES);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<ActivityRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                } 
                return response; 
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }

        public async Task<UserRoleRespObj> GetUserRolesAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new UserRoleRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                var result = await gosGatewayClient.GetAsync(ApiRoutes.IdentitySeverRequests.GET_THIS_ROLES);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<UserRoleRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    throw new Exception($"{response}");
                } 
                return response;
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                throw ex;
                #endregion
            }
        }

       

    }
}
