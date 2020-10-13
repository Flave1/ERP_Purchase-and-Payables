using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Queries.Finanace;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Contracts.Response.Payment; 
using Puchase_and_payables.Contracts.V1;
using System; 
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Puchase_and_payables.Handlers.Report.PayableDaysCalcultion.PayableDaysCalcultionHandler;

namespace Puchase_and_payables.Requests
{
    public class FinanceServerRequest : IFinanceServerRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILoggerService _logger;
        public FinanceServerRequest(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILoggerService logger)
        {
            _httpClientFactory = httpClient;
            _accessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<InvoiceRegRespObj> CreateUpdateInvoiceAsync(InvoiceRegObj request)
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var jsonContent = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = new InvoiceRegRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } }; 
                 

                var result = await gosGatewayClient.PostAsync(ApiRoutes.FinanceServerRequest.CREATE_INVOICE, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<InvoiceRegRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    return response;  
                }

                if (!response.Status.IsSuccessful)
                {
                    response.Status = response.Status;
                    return response;
                }
                response.InvoiceId = response.InvoiceId;
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

        public async Task<BanksRespObj> GetAllBanksAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new BanksRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };


                var result = await gosGatewayClient.GetAsync(ApiRoutes.FinanceServerRequest.GET_BANKS);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<BanksRespObj>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    return response;
                }

                if (!response.Status.IsSuccessful)
                {
                    response.Status = response.Status;
                    return response;
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

        public async Task<SubGlRespObj> GetAllSubglAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new SubGlRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

                SubGlRespObj resultObj = new SubGlRespObj();

                var result = await gosGatewayClient.GetAsync(ApiRoutes.CreditServerEndpoint.SUB_GLS);
                var resultString = await result.Content.ReadAsStringAsync();
                resultObj = JsonConvert.DeserializeObject<SubGlRespObj>(resultString);
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
                response.SubGls = resultObj.SubGls;
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
         
        public async Task<FinancialTransactionRegObj> PassEntryAsync(FinancialTransaction request)
        {
            var response = new FinancialTransactionRegObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

            try
            { 
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var jsonContent = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                 

                var result = await gosGatewayClient.PostAsync(ApiRoutes.FinanceServerRequest.PASS_ENTRY, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<FinancialTransactionRegObj>(resultString);
                if (!result.IsSuccessStatusCode)
                { 
                    return response;
                    //throw new Exception($"{response}");
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

        public async Task<PaidRespObj> MakePaymentsAsync(TransferObj request)
        {
            var response = new PaidRespObj();
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var jsonContent = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await gosGatewayClient.PostAsync(ApiRoutes.FinanceServerRequest.MAKE_TRANSFER, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<PaidRespObj>(resultString);
                return res;
            } 
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                response.status = "failed";
                return response;
                #endregion
            }
        }
 
        public async Task<AccountNumberVerification> VerifyAccountNumber(VerifyAccount request)
        {
            var response = new AccountNumberVerification();
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var jsonContent = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await gosGatewayClient.PostAsync(ApiRoutes.FinanceServerRequest.VERIFY_ACCOUNT_NUMBER, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AccountNumberVerification>(resultString);
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                response.status = "failed";
                return response;
                #endregion
            }
        }

        public async Task<FinancialEntriesResp> GetFinTransactions(ReportGls gls)
        {
            var response = new FinancialEntriesResp { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var jsonContent = JsonConvert.SerializeObject(gls);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await gosGatewayClient.PostAsync(ApiRoutes.FinanceServerRequest.GET_ALL_FINANCIAL_ENTRY, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<FinancialEntriesResp>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    return response;
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
