using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace SimOS
{
    class Disk
    {
        public int[] data;
        //Pointer points to next empty data slot (32-bit) 
        public int pointer = 0;
        converter convert = new converter(); 

        //Creates disk size with 65,536 bits 
        public Disk()
        {
            data = new int[65536];
        }

        //Creates disk with data size from user 
        public Disk(int words)
        {
            words *= 32;
            data = new int[words]; 
        }

        //Enters data into disk 
        public void enterData(string hexadecimal)
        {
            //Converts hexadecimal string to binary char[] 
            char[] bin = returnBinary(hexadecimal); 

            for (int i = 0; i < bin.Length; i++)
            {
                //data[i] - '0' is converting char to integer 
                data[i + (pointer * 32)] = bin[i] - '0'; 
            }
            this.pointer++; 
        }

        //Returns binary from hexadecimal as a char[] 
        private char[] returnBinary (string hexadecimal)
        {
            char[] hexChar = hexadecimal.ToCharArray();
            string bin = "";
            for (int i = 2; i < hexChar.Length; i++)
            {
                bin += convert.HexToBin(hexChar[i]);
            }

            //Returning binary instruction 
            return bin.ToCharArray();
        }

        public void printData(int wordIndex)
        {
            for (int i = wordIndex * 32; i < 32; i++)
            {
                Console.Write(data[i]); 
            }
            Console.WriteLine(""); 
        }

        public int getLength() { return data.Length;  }

        //Prints every line added to disk 
        public void printAddedAll()
        {
            for (int i = 0; i < 32 * pointer; i++)
            {
                if (i % 32 == 0 && i != 0)
                {
                    Console.WriteLine(""); 
                }
                Console.Write(data[i]); 
            }
        }

        //Prints every single line from disk 
        public void printAll()
        {
            for (int i = 0; i < data.Length; i++) 
            {
                Console.Write(data[i]);  
            }
        }
    }
}