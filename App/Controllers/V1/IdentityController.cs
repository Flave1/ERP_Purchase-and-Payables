using System;
using System.Linq; 
using System.Threading.Tasks;
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Contracts.Commands.Supplier;
using Puchase_and_payables.Contracts.Response;
using Puchase_and_payables.Contracts.V1;
using Puchase_and_payables.Handlers.Identity;
using Puchase_and_payables.Requests;

namespace Puchase_and_payables.Controllers.V1
{
    public class PPIdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly ILoggerService _loggerService;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly ISupplierRepository _supRepo;
        private readonly IMediator _mediator;
          
        public PPIdentityController(
            IIdentityService identityService, 
            ILoggerService loggerService, 
            IIdentityServerRequest serverRequest,
            IMediator mediator,
            ISupplierRepository supplierRepository)
        {
            _serverRequest = serverRequest;
            _identityService = identityService;
            _loggerService = loggerService;
            _supRepo = supplierRepository;
            _mediator = mediator;
        } 
        [HttpPost(ApiRoutes.Identity.IDENTITYSERVERLOGIN)]
        public async Task<IActionResult> IdentityServerLogin([FromBody] UserLoginReqObj request)
        {
            var authResponse = await _serverRequest.IdentityServerLoginAsync(request.UserName, request.Password);
            if (authResponse.Token == null)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = authResponse.Status.IsSuccessful,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = authResponse?.Status?.Message?.FriendlyMessage,
                            TechnicalMessage = authResponse?.Status?.Message?.TechnicalMessage
                        }
                    }
                });
            } 
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }



        [HttpPost(ApiRoutes.Identity.REGISTER)]
        public async Task<IActionResult> REGISTER([FromBody] RegistrationCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.Identity.LOGIN)]
        public async Task<IActionResult> LOGIN([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost(ApiRoutes.Identity.CONFIRM_CODE)]
        public async Task<IActionResult> CONFIRM_CODE([FromQuery] ConfirmEmailCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        

        //[HttpPost(ApiRoutes.Identity.LOGIN)]
        //public async Task<IActionResult> Login([FromBody] UserLoginReqObj request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new AuthFailedResponse
        //        {
        //            Status = new APIResponseStatus
        //            {
        //                IsSuccessful = false,
        //                Message = new APIResponseMessage
        //                {
        //                    FriendlyMessage = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)).FirstOrDefault(),
        //                }
        //            }
        //        });
        //    }

        //    var authResponse = await _identityService.LoginAsync(request.UserName, request.Password);
        //    if (!authResponse.Success && !authResponse.IsFirstLogin)
        //    {
        //        return BadRequest(new AuthFailedResponse
        //        {
        //            ///Errors = authResponse.Errors
        //            Status = authResponse.Status
        //        });
        //    }

        //    return Ok(new AuthSuccessResponse
        //    {
        //        Token = authResponse.Token,
        //        RefreshToken = authResponse.RefreshToken
        //    });
        //}

        //[HttpPost(ApiRoutes.Identity.REFRESHTOKEN)]
        //public async Task<IActionResult> Refresh([FromBody] UserRefreshTokenReqObj request)
        //{

        //    var authResponse = await _identityService.RefreshTokenAsync(request.RefreshToken, request.Token);
        //    if (!authResponse.Success)
        //    {
        //        return BadRequest(new AuthFailedResponse
        //        {
        //            //Errors = authResponse.Errors
        //            Status = authResponse.Status
        //        });
        //    }

        //    return Ok(new AuthSuccessResponse
        //    {
        //        Token = authResponse.Token,
        //        RefreshToken = authResponse.RefreshToken,
        //        Status = new APIResponseStatus
        //        {
        //            IsSuccessful = true,
        //        }
        //    });
        //}


        //[HttpPost(ApiRoutes.Identity.CHANGE_PASSWORD)]
        //public async Task<IActionResult> ChangePassword([FromBody] ChangePassword request)
        //{
        //    var authResponse = await _identityService.ChangePasswsord(request);
        //    if (!authResponse.Success)
        //    {
        //        return BadRequest(new AuthFailedResponse
        //        {
        //            Status = authResponse.Status
        //        });
        //    }
        //    return Ok(new AuthSuccessResponse
        //    {
        //        Token = authResponse.Token,
        //        RefreshToken = authResponse.RefreshToken,
        //        Status = new APIResponseStatus { IsSuccessful = true }
        //    });
        //}

        //[HttpPost(ApiRoutes.Identity.CONFIRM_EMAIL)]
        //public async Task<IActionResult> ConfirmEmail([FromBody] ConfirnmationRequest request)
        //{

        //    try
        //    {
        //        if (request.Email.Length < 1)
        //        {
        //            return BadRequest(new ConfirnmationResponse
        //            {
        //                Status = new APIResponseStatus
        //                {
        //                    IsSuccessful = false,
        //                    Message = new APIResponseMessage
        //                    {
        //                        FriendlyMessage = "Email Required change password"
        //                    }

        //                }
        //            });
        //        }


        //        var userExist = await _identityService.CheckUserAsync(request.Email);
        //        if (!userExist)
        //        {

        //            return BadRequest(new ConfirnmationResponse
        //            {
        //                Status = new APIResponseStatus
        //                {
        //                    IsSuccessful = false,
        //                    Message = new APIResponseMessage
        //                    {
        //                        FriendlyMessage = "Email not found"
        //                    }

        //                }
        //            });
        //        }
        //        var response = await _identityService.ConfirmEmailAsync(request);
        //        if (!response.Status.IsSuccessful)
        //        {
        //            return BadRequest(new ConfirnmationResponse
        //            {
        //                Status = new APIResponseStatus
        //                {
        //                    IsSuccessful = false,
        //                    Message = new APIResponseMessage
        //                    {
        //                        FriendlyMessage = response.Status.Message.FriendlyMessage
        //                    }

        //                }
        //            });
        //        }
        //        return Ok(new ConfirnmationResponse
        //        {

        //            Status = new APIResponseStatus
        //            {
        //                IsSuccessful = true
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _loggerService.Error(ex.Message ?? ex.InnerException.Message);
        //        return BadRequest(ex.Message ?? ex.InnerException.Message);
        //    }

        //}

        //[HttpPost(ApiRoutes.Identity.CONFIRM_CODE)]
        //public async Task<IActionResult> ConfirmationCode([FromBody] ConfirnmationRequest request)
        //{

        //    var userExist = await _identityService.VerifyAsync(request.Email, request.Code);
        //    if (!userExist.Status.IsSuccessful)
        //    {
        //        return BadRequest(new ConfirnmationResponse
        //        {
        //            Status = userExist.Status
        //        });
        //    }
        //    return Ok(new ConfirnmationResponse
        //    {
        //        Email = request.Email,
        //        Status = new APIResponseStatus
        //        {
        //            IsSuccessful = true,
        //        }
        //    });
        //}

        public string token { get; set; }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Identity.FETCH_USERDETAILS)]
        public async Task<ActionResult<UserDataResponseObj>> GetUserProfile()
        {
            string userId = HttpContext.User?.FindFirst(c => c.Type == "userId").Value;

            var profile = await _identityService.FetchLoggedInUserDetailsAsync(userId);

            if (!profile.Status.IsSuccessful)
            {
                return BadRequest(profile.Status);
            }
            var supplierDetail = await _supRepo.GetSupplierByEmailAsync(profile.Email);
            if(supplierDetail != null)
            {
                profile.SupplierId = supplierDetail.SupplierId;
            }
            return Ok(profile);
        }



    }
}