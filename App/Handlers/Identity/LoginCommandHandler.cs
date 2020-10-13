using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Puchase_and_payables.AuthHandler;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Helper.Extensions;
using System; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Identity
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly DataContext _dataContext;
        private readonly IIdentityService _identityService;
        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILoggerService loggerService,
            IIdentityService identityService,
            DataContext dataContext)
        {
            _userManager = userManager;
            _identityService = identityService;
            _logger = loggerService;
            _dataContext = dataContext;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = new AuthResponse { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
 
                var result = await _identityService.LoginAsync(user);
   
                response.Token = result.Token;
                response.RefreshToken = result.RefreshToken;
                return response;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage = ex?.Message ?? ex?.InnerException?.Message;
                response.Status.Message.TechnicalMessage = ex.ToString();
                return response;
            }
        }
        public async Task<bool> SendAndStoreConfirmationCode(string code, string email)
        {
            try
            {
                var thisUser = await _userManager.FindByEmailAsync(email);
                var saved = await _dataContext.SaveChangesAsync();
                return saved > 0;
            }
            catch (Exception ex)
            {
                var errorId = ErrorID.Generate(4);
                _logger.Error($"{errorId}   Error Message{ ex?.Message ?? ex?.InnerException?.Message}");
                return false;
            }
        }

    }

}
