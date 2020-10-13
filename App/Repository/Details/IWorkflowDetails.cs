using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Data;
using Puchase_and_payables.DomainObjects.Approvals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.Repository.Details
{
    public interface IWorkflowDetailService
    {
        Task<bool> AddUpdateApprovalDetailsAsync(cor_approvaldetail model);
        Task<IEnumerable<cor_approvaldetail>> GetApprovalDetailsAsync(int TargetId, string token);
    }

    public class WorkflowDetailService : IWorkflowDetailService
    {
        private readonly DataContext _dataContext;
        public WorkflowDetailService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<bool> AddUpdateApprovalDetailsAsync(cor_approvaldetail model)
        {
            if (model.ApprovalDetailId > 0)
            {
                var itemToEdit = await _dataContext.cor_approvaldetail.FindAsync(model.ApprovalDetailId);
                _dataContext.Entry(itemToEdit).CurrentValues.SetValues(model);
            }
            else
                await _dataContext.cor_approvaldetail.AddAsync(model);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<cor_approvaldetail>> GetApprovalDetailsAsync(int targetId, string token)
        {
            var item = await _dataContext.cor_approvaldetail.Where(s => s.TargetId == targetId && s.WorkflowToken.Trim().ToLower() == token.Trim().ToLower()).OrderByDescending(s => s.Date).ToListAsync();
            return item;
        }
    }
}
