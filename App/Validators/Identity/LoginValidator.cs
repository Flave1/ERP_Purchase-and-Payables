using FluentValidation;
using GOSLibraries.GOS_Financial_Identity;
using Microsoft.AspNetCore.Identity;
using Puchase_and_payables.Contracts.Commands.Supplier;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Handlers.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators.Identity
{
    public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RegistrationCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(a => a.Email).NotEmpty().EmailAddress(); 
            RuleFor(x => x.Password)
                .NotNull()
                .MinimumLength(8)
                .MaximumLength(16)
                .MustAsync(IsPasswordCharactersValid).WithMessage("Invalid Password");
            
            RuleFor(c => c).MustAsync(UserExist).WithMessage("User With This Email Already Exist");
               // .MustAsync(IsValidPassword).WithMessage("User/Password Combination is wrong");
        }

        
        private async Task<bool> UserExist(RegistrationCommand request, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return await Task.Run(() => true);
            }
            return await Task.Run(() => false);
        }
        private async Task<bool> IsPasswordCharactersValid(string password, CancellationToken cancellationToken)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return await Task.Run(() => hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password));
        }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public LoginCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Password)
                .NotNull()
                .MinimumLength(8)
                .MaximumLength(16)
                .MustAsync(IsPasswordCharactersValid).WithMessage("Invalid Password");
            RuleFor(a => a.UserName).NotEmpty();
            RuleFor(c => c).MustAsync(UserExist).WithMessage("User does not exist")
                .MustAsync(IsValidPassword).WithMessage("User/Password Combination is wrong");
        }

        private async Task<bool> IsValidPassword(LoginCommand request, CancellationToken cancellation)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                var isValidPass = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!isValidPass)
                {
                    return await Task.Run(() => false);
                }
            }
            else
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private async Task<bool> UserExist(LoginCommand request, CancellationToken cancellation)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private async Task<bool> IsPasswordCharactersValid(string password, CancellationToken cancellationToken)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return await Task.Run(() => hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password));
        }
    }
}
