using FluentValidation;
using Puchase_and_payables.Contracts.Commands.Purchase;
using Puchase_and_payables.Contracts.Response.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Validators.Purchase
{
    public class AddUpdateRequisitionNoteCommandValid : AbstractValidator<AddUpdateRequisitionNoteCommand>
    {
        public AddUpdateRequisitionNoteCommandValid()
        {
            RuleFor(f => f.Description)
                .NotEmpty();
            RuleFor(d => d.RequestBy)
                .NotEmpty();
            RuleFor(c => c.Total)
                .NotEmpty();
            RuleFor(d => d.DepartmentId)
                .NotEmpty()
                .WithMessage("Department Reqquired");
            RuleFor(c => c.PRNDetails)
                .MustAsync(PrnDetailsMustNotBeEmpty).WithMessage("Error Occurred! please check prn details field");
            //RuleFor(a => a.ExpectedDeliveryDate).MustAsync(ExpectedDeliveryDateIsValid).WithMessage("Invalid Expected date of Delivery");

        }

        //private async Task<bool> ExpectedDeliveryDateIsValid(DateTime? deliveryDate, CancellationToken token)
        //{
        //    if(deliveryDate.Value.Date.Year < DateTime.UtcNow.Date)
        //    {
        //        return await Task.Run(() => false);
        //    }
        //    return await Task.Run(() => true);
        //}
        private async Task<bool> PrnDetailsMustNotBeEmpty(List<PRNDetailsObj> prns, CancellationToken token)
        {
            foreach(var item in prns)
            {
                try
                {
                    if (string.IsNullOrEmpty(item.Description) ||
                    item.SubTotal < 1 || item.Quantity < 1 ||
                    item.SubTotal < 1 || item.SuggestedSupplierId.Count() < 1 ||
                    item.UnitPrice < 1) { return false; }
                     
                }
                catch (Exception ex)
                {
                    return false;
                } 
            }
            return await Task.Run(() => true);
        }
    }
}
