using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Supplier
{
    public class cor_financialdetail : GeneralEntity
    {
        [Key]
        public int FinancialdetailId { get; set; }
        public string BusinessSize { get; set; }
        public string Year { get; set; }
        public string Value {get;set;} 
        public int SupplierId { get; set; } 
    }
     
}
