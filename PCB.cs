using System;
using System.IO;
using System.Reflection;

namespace SimOS
{
    class PCB
    {
        //CpuID 
        public int processID;
        public int programCounter;
        public int priority;
        public int wordCount;
        public int diskAddress;
        public int ramBaseAddress;
        public int inputBuffer;
        public int inputBufferAddress;
        public int outputBuffer;
        public int outputBufferAddress;
        public int tempBuffer;
        public int tempBufferAddress;
        public int spaceReq; //How many total BITS process must take up in disk and memory
        public int ioOps; //How many I/O operations per each process 
        public int pageCount;
        public page[] pageTable;
        public string core = "";
        public Boolean complete; //For testing - tells us if job has been executed by CPU
        public TimeSpan ramLoadTime; //Time at which job is loaded into RAM
        public TimeSpan startTime; //Time at which CPU begins work on job
        public TimeSpan timeCompleted; //Time at which CPU finishes work on job
        public TimeSpan waitTime; //How much time job waited in RAM before being picked up by CPU
        public TimeSpan executionTime; //How long CPU took to execute job  

        public PCB()
        {
            this.ramBaseAddress = -1;
            this.programCounter = -1;
            this.complete = false;
        }

        public void showInfo()
        {
            Console.WriteLine("Process ID: " + this.processID);
            Console.WriteLine("Priority Number: " + this.priority);
            Console.WriteLine("Word Count: " + this.wordCount);
            Console.WriteLine("Space requirement (in bits): " + this.spaceReq);
            Console.WriteLine("Starting Disk Address: " + this.diskAddress);
            if (this.ramBaseAddress != -1)
            {
                Console.WriteLine("Starting Memory Address: " + this.ramBaseAddress);
            }
            else
            {
                Console.WriteLine("Starting Memory Address: Process not yet in memory.");
            }
            if (this.programCounter != -1)
            {
                Console.WriteLine("Program Counter: " + this.programCounter);
            }
            else
            {
                Console.WriteLine("Program Counter: This program is not yet in memory.");
            }
            Console.WriteLine("Input Buffer Size: " + this.inputBuffer);
            Console.WriteLine("Output Buffer Size: " + this.outputBuffer);
            Console.WriteLine("Temporary Buffer Size: " + this.tempBuffer);
            Console.WriteLine("Input Buffer Starting Address: " + this.inputBufferAddress);
            Console.WriteLine("Output Buffer Starting Address: " + this.outputBufferAddress);
            Console.WriteLine("Temporary Buffer Starting Address " + this.tempBufferAddress);
            Console.WriteLine("Complete? " + this.complete);

        }

        public void showMetrics()
        {
            Console.WriteLine("Job number " + this.processID + ":");
            Console.WriteLine("# I/O operations: " + this.ioOps);
            Console.WriteLine("Wait time: " + this.waitTime.Milliseconds + "ms");
            Console.WriteLine("Execution time: " + this.executionTime + " seconds");
        }

        public void showDiskInfo(Disk disk)
        {
            for (int i = this.diskAddress; i <= this.diskAddress + this.spaceReq; i++)
            {
                Console.Write(disk.data[i]);
            }
        }

        public void displayCoreDump()
        {
            string temp = "";
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = folder + "/coreDump.txt";
            using (StreamWriter file = new StreamWriter(filepath, true))
            {
                file.WriteLine("Job # " + this.processID + " binary section");
                int count = 1;
                for (int i = 0; i < this.wordCount + this.inputBuffer + this.outputBuffer + this.tempBuffer; i++)
                {
                    temp = "Word " + (i + 1) + ": ";
                    if (i < 9)
                    {
                        temp = "Word " + (i + 1) + ":  ";
                    }
                    if (i == wordCount)
                    {
                        file.WriteLine("Input/Output/Buffer Section");
                    }
                    if (i >= wordCount)
                    {
                        if (count < 10)
                        {
                            temp = "Word " + (i + 1 - wordCount) + ":  ";
                        }
                        else
                        {
                            temp = "Word " + (i + 1 - wordCount) + ": ";
                        }
                        count++;
                    }
                    for (int j = 0; j < 32; j++)
                    {
                        temp += core[i + j];
                    }
                    file.WriteLine(temp);
                }

            }
        }

        public void getPageCount()
        {
            decimal value = decimal.Ceiling((decimal)this.wordCount / 4);
            this.pageCount = (int)value;
           
        }

        public void getPageInfo()
        {
            for (int i = this.diskAddress; i < this.diskAddress + this.wordCount * 32; i += 32)
            {
                //i is a starting disk address for each individual word
                if (this.pageTable[this.getAvailablePage()].wordCount == 0)
                {
                    this.pageTable[this.getAvailablePage()].diskBaseAddress = i;
                }
                this.pageTable[this.getAvailablePage()].wordCount++;
            }
        }

        public void showPageTable()
        {
            Console.WriteLine("Process number " + this.processID);
            for (int i = 0; i < this.pageTable.Length; i++)
            {
                Console.Write("Page " + i + ": ");
                if(this.pageTable[i].frameNumber != -1)
                {
                    Console.Write("Allocated to frame " + this.pageTable[i].frameNumber + " ");
                }
                Console.Write("Word Count: " + this.pageTable[i].wordCount + " ");
                Console.WriteLine("Disk Address of Page: " + this.pageTable[i].diskBaseAddress);
                for(int j = 0; j < 4; j++)
                {
                    if(this.pageTable[j].valid == true)
                    {
                        Console.WriteLine("Page"+j+"in memory");
                    }
                }

            }
        }

        public void initializePageTable()
        {
            for (int i = 0; i < this.pageTable.Length; i++)
            {
                this.pageTable[i] = new page();
            }
        }

        public int getAvailablePage()
        {
            int result = 0;
            while (this.pageTable[result].wordCount == 4)
            {
                result++;
            }
            return result;
        }

    }
}
