using System;
using System.Collections.Generic;
using System.Text;

namespace SimOS
{
    public class RAM
    {
        public int[] data;
        public double percentageUsed;
        public double overallUsed;
        public int iterations;
        public frame[] frameList;
        //Creates RAM with size of 32,768 
        public RAM()
        {
            data = new int[32768];
            frameList = new frame[256]; //There are 256 possible frames of 4 words each
            for (int i = 0; i < 32768; i++) //Values of -1 indicate the memory bit is available
            {
                data[i] = -1;

            }
            initializeFrameList();
        }

        //Creates RAM with size from user input 
        public RAM(int words)
        {
            words *= 32;
            data = new int[words];
        }

        public void initializeFrameList()
        {
            int point = 0;
            for(int i = 0; i < data.Length; i+=128)
            {
                this.frameList[point] = new frame(i);
                point++;
            }
        }

        public void display()
        {
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write(data[i]);
            }
        }

        public void calculateUsed() //Calculates percentage of RAM that has been used
        {
            double used = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != -1)
                {
                    used++;
                }
            }
            this.percentageUsed = used / data.Length * 100;
            this.overallUsed += this.percentageUsed;
            this.iterations++;
            this.overallUsed /= this.iterations;
        }

        public void getUsed()
        {
            Console.WriteLine("Percentage of RAM used on average throughout all 30 jobs: " + this.overallUsed + "%");
        }

        public String coreDump(int startIndex, int endIndex)
        {
            string result = "";
            for (int i = startIndex; i < endIndex; i++)
            {
                result += data[i];
            }
            result += '\n';
            return result;
        }

        public void showFrameList()
        {
            for(int i = 0; i < frameList.Length;i++)
            {
                Console.WriteLine("Frame " + i + " starting address "+frameList[i].startAddress);
            }
        }

        public int getFreeFrame()
        {
            int result = 0;
            while(frameList[result].available == false)
            { 
                result++;
                if(result == frameList.Length)
                {
                    return result;
                }
            }
            return result;
        }

    }
}
