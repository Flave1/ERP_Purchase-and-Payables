using FluentValidation;
using GODPAPIs.Contracts.Commands.Supplier; 

namespace GODP.APIsContinuation.Validations
{
    public class UpdateSupplierDocumentCommandVal: AbstractValidator<UpdateSupplierDocumentCommand>
    {
        public UpdateSupplierDocumentCommandVal()
        { 
            RuleFor(w => w.Description).NotEmpty().MinimumLength(4); 
            RuleFor(r => r.ReferenceNumber).NotEmpty();
        }
    }
}
