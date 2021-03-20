using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Timers;
//Class converts nunbers between binary, hexadecimal, and decimal 
namespace SimOS
{
    class converter
    {
        //Converts hexadecimal to binary from string 

        /*Note: integer is too small to support 32 digit numbers, 
         Only input a hexadecimal character and return 4 digit integer 
         */
        public string HexToBin(char hex)
        {
            if (hex == '0')
                return "0000";
            else if (hex == '1')
                return "0001";
            else if (hex == '2')
                return "0010";
            else if (hex == '3')
                return "0011";
            else if (hex == '4')
                return "0100";
            else if (hex == '5')
                return "0101";
            else if (hex == '6')
                return "0110";
            else if (hex == '7')
                return "0111";
            else if (hex == '8')
                return "1000";
            else if (hex == '9')
                return "1001";
            else if (hex == 'A')
                return "1010";
            else if (hex == 'B')
                return "1011";
            else if (hex == 'C')
                return "1100";
            else if (hex == 'D')
                return "1101";
            else if (hex == 'E')
                return "1110";
            else if (hex == 'F')
                return "1111";
            else
                return "-1";

        }

        //Function converts hexadecimal to decimal
        public int HexToDecimal(string hexadecimal)
        {
            char[] hexChar = hexadecimal.ToCharArray(); 
            int dec = 0;
            string bin = ""; 
            for (int i = 0; i < hexChar.Length; i++)
            {
                bin += HexToBin(hexChar[i]);  
            } 
            bin.ToCharArray();
            int y = bin.Length - 1; 
            for (int i = 0; i < bin.Length; i++)
            {
                dec += (int)Math.Pow(2, y) * (bin[i] - '0');
                y--; 
            }
            return dec; 
        }

    }
}
