﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puchase_and_payables.Helper.Extensions
{
    public static class ConfirmationCode
    {
        private static Random random = new Random();
        public static string Generate()
        {
            const string chars = "!@#$//||%&*ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
