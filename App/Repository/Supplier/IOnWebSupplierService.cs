using GODP.APIsContinuation.DomainObjects.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Repository.Supplier
{
    public interface IOnWebSupplierService
    {
        Task<IEnumerable<cor_supplier>> OnWebSupplierGetAsync(string email);
    }
}
