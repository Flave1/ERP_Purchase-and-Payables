using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Puchase_and_payables.Helper.Extensions
{
    public static class SupplierNumber
    {
        private static Random random = new Random();
        public static string Generate(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public static class PRNNumber
    {
        private static Random random = new Random();
        public static string Generate(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}