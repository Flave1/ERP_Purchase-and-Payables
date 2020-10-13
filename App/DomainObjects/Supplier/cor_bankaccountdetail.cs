using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Supplier
{
    public class cor_bankaccountdetail : GeneralEntity
    {
        [Key]
        public int BankAccountDetailId { get; set; }
        public int SupplierId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BVN { get; set; }
        public int Bank { get; set; }
    }
}
