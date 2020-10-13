using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Contracts.Queries.Finanace
{
    public class TransferObj
    {
        public string account_bank { get; set; }
        public string account_number { get; set; }
        public long amount { get; set; }
        public string narration { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }
        public string callback_url { get; set; }
        public string debit_currency { get; set; }
    }

    public class PaidRespObj
    {
        public string status { get; set; }
        public string message { get; set; }
    }

    public class AccountNumberVerification
    {
        public string status { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }

    public class VerifyAccount
    {
        public string account_number { get; set; }
        public string account_bank { get; set; }
    }

}
