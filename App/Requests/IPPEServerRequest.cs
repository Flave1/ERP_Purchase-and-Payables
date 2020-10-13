using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Response.FinanceServer;
using Puchase_and_payables.Contracts.Response.Payment;
using Puchase_and_payables.Contracts.Response.PPEServer;
using Puchase_and_payables.Contracts.Response.Purchase;
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
    public interface IPPEServerRequest
    {
        Task<UpdatePPELPORegResp> CreateUpdateLPOForAdditionAsync(List<UpdatePPELPO> request);
        Task<AssetClassificationRespObj> GetAllPPEClassificationsAsync();
    }

    public class PPEServerRequest : IPPEServerRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILoggerService _logger;
        public PPEServerRequest(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILoggerService logger)
        {
            _httpClientFactory = httpClient;
            _accessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<UpdatePPELPORegResp> CreateUpdateLPOForAdditionAsync(List<UpdatePPELPO> request)
        {
            var response = new UpdatePPELPORegResp { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };

            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var jsonContent = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                 

                var result = await gosGatewayClient.PostAsync(ApiRoutes.PPEServerEndpoint.UPDATE_LPO, byteContent);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<UpdatePPELPORegResp>(resultString);
                if (!result.IsSuccessStatusCode)
                {
                    response.Status.IsSuccessful = false;
                    response.Status.Message.FriendlyMessage = $"{result.ReasonPhrase}  {(int)result.StatusCode}  {result.Content}";
                    return response;
                    //throw new Exception($"{response}");
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

        public async Task<AssetClassificationRespObj> GetAllPPEClassificationsAsync()
        {
            try
            {
                var gosGatewayClient = _httpClientFactory.CreateClient("GOSDEFAULTGATEWAY");
                string authorization = _accessor.HttpContext.Request.Headers["Authorization"];
                gosGatewayClient.DefaultRequestHeaders.Add("Authorization", authorization);

                var response = new AssetClassificationRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
                 
                var result = await gosGatewayClient.GetAsync(ApiRoutes.PPEServerEndpoint.GET_ALL_ADDITIONS);
                var resultString = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<AssetClassificationRespObj>(resultString);
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
    }
}
