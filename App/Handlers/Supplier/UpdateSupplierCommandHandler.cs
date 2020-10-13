using GODP.APIsContinuation.DomainObjects.Supplier;
using GODP.APIsContinuation.Repository.Interface;
using GODPAPIs.Contracts.Commands.Supplier;
using GODPAPIs.Contracts.RequestResponse.Supplier; 
using MediatR;  
using System; 
using System.Net.Http; 
using System.Threading;
using System.Threading.Tasks;
using Puchase_and_payables.Data;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response; 
using Puchase_and_payables.Contracts.Response.ApprovalRes; 
using Puchase_and_payables.Requests;
using Microsoft.AspNetCore.Identity;
using Puchase_and_payables.DomainObjects.Auth;
using Puchase_and_payables.Contracts.Response.IdentityServer;
using GOSLibraries.GOS_MAIL_BOX;
using GOSLibraries;
using System.Collections.Generic;
using System.Linq;
using Support.SDK; 
using Microsoft.Extensions.Configuration;
using GOSLibraries.URI;

namespace GODP.APIsContinuation.Handlers.Supplier
{
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, SupplierRegRespObj>
    {
        private readonly ISupplierRepository _supplierRepo;
        private readonly ILoggerService _logger; 
        private readonly IIdentityServerRequest _serverRequest; 
        public GoForApprovalRespObj res;
        private readonly IUriService _uri;
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBaseURIs _uRIs;
        public UpdateSupplierCommandHandler(
            ISupplierRepository supplierRepository, 
            ILoggerService loggerService,  
            IIdentityServerRequest serverRequest,
            DataContext dataContext,
            IBaseURIs uRIs,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IUriService uriService)
        {
            _logger = loggerService; 
            _supplierRepo = supplierRepository; 
            _serverRequest = serverRequest;
            _userManager = userManager;
            _dataContext = dataContext;
            _configuration = configuration;
            _uri = uriService;
            _uRIs = uRIs;
        }
        public async Task<SupplierRegRespObj> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var apiResponse = new SupplierRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            try
            {
                ApplicationUser supplierUserAccount = new ApplicationUser(); 
                var user = await _serverRequest.UserDataAsync();
                if(user.StaffId < 1 )
                {
                    supplierUserAccount = await _userManager?.FindByEmailAsync(request.Email.Trim());
                } 
                cor_supplier supp = new cor_supplier();
                if (user.StaffId < 1)
                { 
                    if (request.SupplierId > 0)
                    {
                        supp = await BuildExistingSupplierObj(request);
                        
                        await _supplierRepo.UpdateSupplierAsync(supp);

                        await CreateUpdteSuppliersUser(supp);
                    }
                    else
                    {
                        supp =  BuildNewSupplierObj(request);
                        await _supplierRepo.AddNewSupplierAsync(supp);
                    }
                    apiResponse.SupplierId = supp.SupplierId;
                    apiResponse.Status.IsSuccessful = true;
                    apiResponse.Status.Message.FriendlyMessage = "Successful";
                    return apiResponse;
                }

                using (var transaction = await _dataContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        if (request.SupplierId > 0)
                        {
                            supp = await BuildExistingSupplierObj(request);
                            try
                            {
                                var id = await CreateUpdteSuppliersUser(supp);
                                supp.UserId = id;
                            }
                            catch (Exception ex)
                            {
                                 await transaction.RollbackAsync();
                                apiResponse.Status.Message.FriendlyMessage = ex.Message;
                                return apiResponse;
                            }
                            await _supplierRepo.UpdateSupplierAsync(supp);
                            await transaction.CommitAsync();
                            apiResponse.SupplierId = supp.SupplierId;
                            apiResponse.Status.IsSuccessful = true;
                            apiResponse.Status.Message.FriendlyMessage = "Successful";
                            return apiResponse;
                        }
                        else
                        {
                            if (supplierUserAccount.Email != null && request.SupplierId < 1)
                            {
                                apiResponse.Status.Message.FriendlyMessage = $"Supplier with this email {request.Email} already exist";
                                return apiResponse;
                            }

                            var newSupplier = BuildNewSupplierObj(request);

                            var id = await CreateUpdteSuppliersUser(newSupplier);

                            newSupplier.UserId = id;
                            await _supplierRepo.AddNewSupplierAsync(newSupplier);
                            await transaction.CommitAsync();
                            apiResponse.SupplierId = newSupplier.SupplierId;
                            apiResponse.Status.IsSuccessful = true;
                            apiResponse.Status.Message.FriendlyMessage = "Supplier Details Saved";
                            return apiResponse;
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        apiResponse.Status.Message.FriendlyMessage = ex.Message;
                        return apiResponse;
                    }
                }
                    
            }
            catch (Exception ex)
            {
                #region Log error to file 
                var errorCode = ErrorID.Generate(4);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new SupplierRegRespObj
                {

                    Status = new APIResponseStatus
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = "Error occured!! Please try again later",
                            MessageId = errorCode,
                            TechnicalMessage = $"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}"
                        }
                    }
                };
                #endregion
            }
        }

        public async Task<cor_supplier> BuildExistingSupplierObj(UpdateSupplierCommand request)
        {
            var supplier = await _supplierRepo.GetSupplierAsync(request.SupplierId);
            if (supplier != null)
            {
                supplier.Address = request.Address;
                supplier.Name = request.Name;
                supplier.Passport = request.Passport;
                supplier.Email = request.Email;
                supplier.PhoneNo = request.PhoneNo;
                supplier.RegistrationNo = request.RegistrationNo;
                supplier.SupplierTypeId = request.SupplierTypeId;
                supplier.CountryId = request.CountryId;
                supplier.UpdatedBy = request.CreatedBy;
                supplier.UpdatedOn = DateTime.Now;
                supplier.Website = request.Website;
                supplier.PostalAddress = request.PostalAddress;
                supplier.TaxIDorVATID = request.TaxIDorVATID;
                supplier.SupplierNumber = request.SupplierNumber;
                supplier.Particulars = request.Particulars;
                supplier.SupplierId = request.SupplierId; 
            }
            return supplier;
        }

        public cor_supplier BuildNewSupplierObj(UpdateSupplierCommand request)
        {
            return  new cor_supplier
            {
                Name = request.Name,
                Address = request.Address,
                PhoneNo = request.PhoneNo,
                Email = request.Email,
                RegistrationNo = request.RegistrationNo,
                SupplierTypeId = request.SupplierTypeId,
                Passport = request.Passport,
                CountryId = request.CountryId,
                ApprovalStatusId = (int)ApprovalStatus.Pending,
                Active = true,
                Deleted = false,
                CreatedBy = request.CreatedBy,
                CreatedOn = DateTime.Now,
                UpdatedBy = request.CreatedBy,
                UpdatedOn = DateTime.Now,
                Website = request.Website,
                PostalAddress = request.PostalAddress,
                TaxIDorVATID = request.TaxIDorVATID,
                SupplierNumber = request.SupplierNumber,
                HaveWorkPrintPermit = request.HaveWorkPrintPermit == 1 ? true : false,
                Particulars = request.Particulars,
                
            };
        }

        private async Task SendMailToUpdatedSuppliersAsync(ApplicationUser user)
        {
            var em = new EmailMessageObj();
            em.ToAddresses.Add(new EmailAddressObj { Address = user.Email });
            em.Subject = "Profile Login Details Updated";
            em.Content = $"Dear {user.FullName} <p>Your supplier account was successfully created </p> <br> Below is your account details" +
                $"by stating your terms which may be negotiable and best for this product and service";
            em.SendIt = true;
            await _serverRequest.SendMessageAsync(em);
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
            em.Subject = "Supplier Account Created";
            em.Content = $"<p style='float:left'> Dear {user.FullName} <br> " +
                $"Congratulations, your account has been successfully created <br> " +
                $"Below is your profile login details<br>" +
                $"<b>Username</b>: {user.Email} <br>" +
                $"<b>Password</b>: Password@1 <br>" +
                $"Please click <a href='{path2}'> here </a>  to access your profile and to complete registration</p>";

            em.SendIt = true;
            em.SaveIt = false;
            em.Template = (int)EmailTemplate.LoginDetails;

            await _serverRequest.SendMessageAsync(em);
        }

        private async Task<string> CreateUpdteSuppliersUser(cor_supplier request)
        {
            ApplicationUser User = new ApplicationUser();
            User = await _userManager.FindByIdAsync(request.UserId);

            if (User != null)
            {
                User.Email = request.Email;
                User.UserName = request.Email;
                User.PhoneNumber = request.PhoneNo;
                User.FullName = request.Name;
                await _userManager.UpdateAsync(User);
            }
            else
            {
                User = new ApplicationUser();
                User.Email = request.Email;
                User.UserName = request.Email;
                User.PhoneNumber = request.PhoneNo;
                User.FullName = request.Name;
                await _userManager.CreateAsync(User, "Password@1");
                await SendMailToNewSuppliersAsync(User);
            }
            return User.Id;
        }

    }
}
