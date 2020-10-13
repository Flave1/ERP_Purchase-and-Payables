
using GODP.APIsContinuation.Repository.Interface;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using GOSLibraries.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Puchase_and_payables.Contracts.Response; 
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Helper.Extensions;
using Puchase_and_payables.Requests;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Puchase_and_payables.AuthHandler
{
   
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILoggerService _logger;
        private readonly IIdentityServerRequest _serverRequest; 
      
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            JwtSettings jwtSettings, 
            TokenValidationParameters tokenValidationParameters,
            DataContext dataContext, 
            RoleManager<IdentityRole> roleManager, 
            ILoggerService loggerService, 
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerRequest serverRequest)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings; 
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;
            _roleManager = roleManager;  
            _logger = loggerService;
            _serverRequest = serverRequest;
        }

        public async Task<AuthenticationResult> LoginAsync(ApplicationUser user)
        {
            return await GenerateAuthenticationResultForUserAsync(user);
        }  

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string token)
        {
            try
            {
                var validatedToken = GetClaimsPrincipalFromToken(token);
                if (validatedToken == null)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Invalid Token"
                            }
                        }
                    };
                }


                var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                     .AddDays(expiryDateUnix);

                if (expiryDateTimeUtc > DateTime.UtcNow)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This Token Hasn't Expired Yet"
                            }
                        }
                    };
                }


                var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;

                var storedRefreshToken = _dataContext.RefreshToken.SingleOrDefault(x => x.Token == refreshToken);

                if (storedRefreshToken == null)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This Refresh Token does not Exist"
                            }
                        }
                    };
                }

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This Refresh Token has Expired"
                            }
                        }
                    };
                }

                if (storedRefreshToken.Invalidated)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This Refresh Token has been Invalidated"
                            }
                        }
                    };
                }

                if (storedRefreshToken.Used)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This Refresh Token has been Used"
                            }
                        }
                    };
                }

                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This Refresh Token Does not match this JWT"
                            }
                        }
                    };
                }

                storedRefreshToken.Used = true;
                _dataContext.Update(storedRefreshToken);
                await _dataContext.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(validatedToken.Claims.SingleOrDefault(x => x.Type == "id").Value);

                return await GenerateAuthenticationResultForUserAsync(user);
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} Ex: {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new AuthenticationResult
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please tyr again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }


        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;
                else
                    return principal;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} <br>Ex: {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                            StringComparison.InvariantCultureIgnoreCase);
        }


        //private async Task SendMail(string email, string code)
        //{
        //    var sm = new EmailMessageObj();
        //    sm.Subject = $"Account Confirmation";
        //    sm.Content = $"This is your account confirnmation code {} <br/>";
        //    sm.ToAddresses.Add(new EmailAddressObj { Address = email, Name = name });
        //    await _serverRequest.SendMessageAsync(sm);

        //}

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id)
            };

                var userClaims = await _userManager.GetClaimsAsync(user);

                claims.AddRange(userClaims);

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var userRole in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole) ?? null);

                    var role = await _roleManager.FindByNameAsync(userRole);

                    if (role == null)
                    {
                        continue;
                    }
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    foreach (var roleClaim in roleClaims)
                    {
                        if (claims.Contains(roleClaim)) continue;
                        claims.Add(roleClaim);
                    }
                }

                var tokenDecriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeSpan),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };


                var token = tokenHandler.CreateToken(tokenDecriptor);

                var refreshToken = new RefreshToken
                {
                    JwtId = token.Id,
                    UserId = user.Id,
                    CreationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddSeconds(6),
                };

                try
                {
                    await _dataContext.RefreshToken.AddAsync(refreshToken);
                    await _dataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = ex.InnerException.Message,
                            }
                        }
                    };
                }

                return new AuthenticationResult
                { 
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.Token,
                };
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new AuthenticationResult
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :{errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }

        }

        public async Task<AuthenticationResult> ChangePasswsord(ChangePassword pass)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(pass.Email);

                var userPassword = await _userManager.CheckPasswordAsync(user, pass.OldPassword);

                if (!userPassword)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "This password is not associated to this account",
                            }
                        }
                    };
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var changepassword = await _userManager.ResetPasswordAsync(user, token, pass.NewPassword);

                if (!changepassword.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = changepassword.Errors.Select(x => x.Description).FirstOrDefault(),
                            }
                        }
                    };
                }

                return await GenerateAuthenticationResultForUserAsync(user);
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new AuthenticationResult
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }

        }

        public async Task<bool> CheckUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null) return true;
            return false;
        }
        public async Task<UserDataResponseObj> FetchLoggedInUserDetailsAsync(string userId)
        {
            try
            {
                UserDataResponseObj profile = new UserDataResponseObj(); 
                var currentUser = await _userManager.FindByIdAsync(userId);

                var supplierData = _dataContext.cor_supplier.FirstOrDefault(a => a.Email.Trim().ToLower() == currentUser.Email.Trim().ToLower());
                if(supplierData != null)
                {
                    profile.PhoneNumber = supplierData.SupplierNumber;
                }
                 
                profile.Email = currentUser.Email;
                profile.UserId = currentUser.Id;
                profile.UserName = currentUser.UserName;
                profile.CustomerName = currentUser.FullName;
                profile.Status = new APIResponseStatus { IsSuccessful = true };

                if (profile == null)
                {
                    return new UserDataResponseObj
                    {

                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Unable to fetch user details"
                            }
                        }
                    };
                }
                return profile;
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new UserDataResponseObj
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please tyr again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }

        public async Task<ConfirnmationResponse> ConfirmEmailAsync(ConfirnmationRequest request)
        {
            try
            {
                var confirmCode = ConfirmationCode.Generate();
                var sent = await SendAndStoreConfirmationCode(confirmCode, request.Email);
                if (!sent)
                    return new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Unable to send mail!! please contact systems administrator"
                            }
                        }
                    };

                return new ConfirnmationResponse
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Please Check your email for email for confirnmation"
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new ConfirnmationResponse
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please tyr again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }

        public async Task<ConfirnmationResponse> VerifyAsync(string email, string code)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "User not Identified"
                            }
                        }
                    };
                }

                var verificationCodeFrmRepo = await _dataContext.ConfirmEmailCodes
                    .FirstOrDefaultAsync(x => x.ConfirnamationTokenCode.Trim().ToLower() == code.Trim().ToLower() 
                && user.Id.ToLower() == x.UserId);

                if (verificationCodeFrmRepo == null)
                {
                    return new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Verification Code Unidentified"
                            }
                        }
                    };
                }
                if (verificationCodeFrmRepo.ExpiryDate < DateTime.Now)
                {
                    return new ConfirnmationResponse
                    {
                        Status = new APIResponseStatus
                        {
                            IsSuccessful = false,
                            Message = new APIResponseMessage
                            {
                                FriendlyMessage = "Verification Code has Expired"
                            }
                        }
                    };
                }
                return new ConfirnmationResponse
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                    }
                };
            }
            catch (Exception ex)
            {
                #region Log error 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new ConfirnmationResponse
                {

                    Status = new APIResponseStatus
                    {
                        IsSuccessful = false,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please tyr again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID :  {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }

        }

        public async Task<bool> SendAndStoreConfirmationCode(string code, string email)
        {
            try
            {
                var thisUser = await _userManager.FindByEmailAsync(email);
                //await _emailService.Send(new EmailMessage
                //{
                //    FromAddresses = new List<EmailAddress>
                //        {
                //            new EmailAddress{ Address = "favouremmanuel433@gmail.com", Name = "Flave Techs"}
                //        },
                //    ToAddresses = new List<EmailAddress>
                //        {
                //            new EmailAddress{ Address = email, Name = thisUser.UserName}
                //        },
                //    Subject = "Account Confirmation",
                //    Content = $"Dear {thisUser.UserName}, <br> Copy and paste this code {code} on the confirmation field to change your password",
                //});

                var userConfirmationCode = new ConfirmEmailCode
                {
                    ConfirnamationTokenCode = code,
                    ExpiryDate = DateTime.Now.AddHours(1),
                    IssuedDate = DateTime.Now,
                    UserId = thisUser.Id
                };
                await _dataContext.ConfirmEmailCodes.AddAsync(userConfirmationCode);
                var saved = await _dataContext.SaveChangesAsync();
                return saved > 0;
            }
            catch (Exception ex)
            {
                var errorId = ErrorID.Generate(4);
                _logger.Error($"SendAndStoreConfirmationCode{errorId}   Error Message{ ex?.Message ?? ex?.InnerException?.Message}");
                return false;
            } 
        }

    }
}
