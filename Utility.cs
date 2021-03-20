using System;
using System.Collections.Generic;
using System.Text;

namespace SimOS
{
    static class Utility
    {
        public static string DecimalToBinary(int num)
        {
            string bin = Convert.ToString(num, 2);
            while (bin.Length < 10)
            {
                bin = "0" + bin;
            }
            return bin;
        }

        public static int BinaryToDecimal(string bin)
        {
            return Convert.ToInt32(bin, 2);
        }
    }
}
