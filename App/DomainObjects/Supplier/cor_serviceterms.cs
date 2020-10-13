using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Supplier
{
    public class cor_serviceterms : GeneralEntity
    {
        [Key]
        public int ServiceTermsId { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
    }
}
