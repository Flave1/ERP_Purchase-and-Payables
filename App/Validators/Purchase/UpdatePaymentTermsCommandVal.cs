using FluentValidation;
using GOSLibraries.Enums;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Handlers.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators.Purchase
{
    public class SaveUpdatePaymentTermsCommandVal : AbstractValidator<SaveUpdatePaymentTermsCommand>
    {
        public SaveUpdatePaymentTermsCommandVal()
        { 
            RuleFor(d => d.BidAndTenderId).NotEmpty();
            RuleFor(d => d.TotalAmount).NotEmpty();
            RuleFor(d => d).MustAsync(NoDuplcatePhaseAsync).WithMessage("Duplicate Phase Detected");
        }
        private async Task<bool> NoDuplcatePhaseAsync(SaveUpdatePaymentTermsCommand request, CancellationToken cancellationToken)
        {
            if (request.Terms.Count() > 0)
            {
                if (request.Terms.GroupBy(q => q.Phase).Any(a => a.Count() > 1))
                {
                    return await Task.Run(() => false);
                }
            }
            return await Task.Run(() => true);
        }
    }
}
