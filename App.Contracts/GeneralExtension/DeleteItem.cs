using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.GeneralExtension
{
    public class DeleteItem
    {
        public int targetId { get; set; }
    }
    public class MultiDeleteItems
    {
        public List<DeleteItem> targetIds { get; set; }
    }
}
