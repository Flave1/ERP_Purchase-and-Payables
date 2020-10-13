using GODP.APIsContinuation.DomainObjects.Supplier;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Repository.Supplier
{
    public class OnWebSupplierService : IOnWebSupplierService
    {
        private readonly DataContext _dataContext;
        public OnWebSupplierService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IEnumerable<cor_supplier>> OnWebSupplierGetAsync(string email)
        {
            return await _dataContext.cor_supplier.Where(s => s.Email.Trim().ToLower() == email.Trim().ToLower()).ToListAsync();
        }
    }
}
