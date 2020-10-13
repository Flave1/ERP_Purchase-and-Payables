using FluentValidation;
using Puchase_and_payables.Handlers.Purchase;  

namespace Puchase_and_payables.Validators.Purchase
{
    public class PaymentValidator : AbstractValidator<SendPaymentInvoiceToApprovalCommand>
    {
        public PaymentValidator()
        {
            RuleFor(w => w.PaymentBankId).NotEmpty().WithMessage("Select Bank to Pay from");
            RuleFor(w => w.PaymentTermId).NotEmpty().WithMessage("Unable not identify phase to pay for");
            RuleFor(w => w.SupplierBankId).NotEmpty().WithMessage("Select Supplier Bank to pay into");
        }
    }
}
