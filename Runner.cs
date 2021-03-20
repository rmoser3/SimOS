using System;
using System.Runtime.InteropServices;

namespace SimOS
{
    class Runner
    {
        static void Main(string[] args)
        {
            //Debug mode will display operations and metrics in the console
            bool debugMode = true;
            Driver drive = new Driver(debugMode, "program-file.txt");

            while (true)
            {
                Console.WriteLine("Debug mode = " + debugMode.ToString());
                Console.WriteLine("One CPU (type 1) or Four CPU (type 2)?");
                string input = Console.ReadLine();
                if (input.Equals("1"))
                {
                    drive.run_1();
                    break;
                }
                else if (input.Equals("2"))
                {
                    drive.run_4();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }
}
