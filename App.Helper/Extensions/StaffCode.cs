using System;
using System.Collections.Generic;
using System.Text;

namespace Puchase_and_payables.Helper.Extensions
{
    public static class LPONumber
    {

        public static string Generate(int count)
        { 
            string zeros = AppendZero(count);
            return $"LPO-{zeros}{count}";
        }

        public static string AppendZero(int ValueInt)
        {
            double value = ValueInt;
            int sign = 0;
            if (value < 0)
            {
                value = -value;
                sign = 0;
            }
            if (value <= 9)
            {
                return "00000".ToString();
            }
            if (value <= 99)
            {
                return "0000".ToString();
            }
            if (value <= 999)
            {
                return "000".ToString();
            }
            if (value <= 9999)
            {
                return "00".ToString();
            }
            if (value <= 99999)
            {
                return "0".ToString();
            }
            else if (value <= 999999)
            {
                return sign.ToString();
            }
            else
            {
                return sign.ToString();
            }
        }
    }
}
