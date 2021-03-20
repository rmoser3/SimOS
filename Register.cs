using System;
using System.Collections.Generic;
using System.Text;

namespace SimOS
{
    class Register
    {
        private int[] registers;
        public Register()
        {
            registers = new int[16];
        }

        public void AssignValueToRegister(int value, int index)
        {
            registers[index] = value;
        }

        public int RetrieveValueFromRegister(int registersIndex)
        {
            return registers[registersIndex];
        }

        public void PrintRegisterContents()
        {
            for (int i = 0; i < registers.Length; i++)
            {
                Console.Write(registers[i]);
            }
            Console.WriteLine();
        }
    }
}
