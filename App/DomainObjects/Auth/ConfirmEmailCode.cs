using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puchase_and_payables.DomainObjects.Auth
{
    public class ConfirmEmailCode : GeneralEntity
    {
        [Key]
        public int cConfirmEmailCodeId { get; set; }
        public string UserId { get; set; }
        public string ConfirnamationTokenCode { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
