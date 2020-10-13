using FluentValidation;
using GODPAPIs.Contracts.Commands.Supplier; 
using Puchase_and_payables.Data;
using Puchase_and_payables.Requests;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GODP.APIsContinuation.Validations
{
    public class UpdateSupplierCommandVal : AbstractValidator<UpdateSupplierCommand>
    {
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        public UpdateSupplierCommandVal(DataContext dataContext, IIdentityServerRequest  identityServer)
        {
            _serverRequest = identityServer;
            _dataContext = dataContext;

            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.PhoneNo).NotEmpty().MinimumLength(11);
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x)
               .MustAsync(NoDuplicateEmail).WithMessage("Supplier With Same Email Already Exist");
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.PostalAddress).NotEmpty();
            RuleFor(x => x.TaxIDorVATID).NotEmpty();
        }
        private async Task<bool> NoDuplicateEmail(UpdateSupplierCommand request, CancellationToken cancellation)
        {
            var user = await _serverRequest.UserDataAsync();
            if(user.StaffId > 0)
            {
                if (request.SupplierId > 0)
                {
                    var otherSupplier = _dataContext.cor_supplier.FirstOrDefault(q => q.Email.Trim().ToLower() == request.Email.Trim().ToLower() && q.SupplierId != request.SupplierId) ?? null;
                    if (otherSupplier == null)
                    {
                        return await Task.Run(() => true);
                    }
                }
                var other = _dataContext.cor_supplier.FirstOrDefault(q => q.Email.Trim().ToLower() == request.Email.Trim().ToLower()) ?? null;
                if (other == null)
                {
                    return await Task.Run(() => true);
                }
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
    }

 
}
