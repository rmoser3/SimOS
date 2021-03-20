using System;
using System.IO;
using System.Reflection;

namespace SimOS
{
    class pcbQueue
    {
        public int size;
        public int ramIterator; //Points at the next process to be loaded into RAM after pcbQueue has been initialized
        public int cpuIterator; //Points at next process to be executed by CPU
        public PCB[] queue;
        public TimeSpan avgWait; //Average wait time for each job
        public TimeSpan avgExecution; //Average completion time for each job 
        public TimeSpan totalExecution;
        converter convert = new converter();


        public pcbQueue(int size)
        {
            this.size = size;
            this.queue = new PCB[size];
        }

        public void showSize()
        {
            Console.WriteLine(size);
        }

        public void setJobInfo(String str, int diskPointer)
        {
            string temp = str;
            int pID;
            int wordCount;
            int priority;

            if (temp[1].Equals(' ') == true) //If the hexadecimal job number is a single digit
            {
                pID = convert.HexToDecimal(temp.Substring(0, 1));
                temp = temp.Substring(2, temp.Length - 2);
            }
            else  //If the hexadecimal job number is two digits
            {
                pID = convert.HexToDecimal(temp.Substring(0, 2));
                temp = temp.Substring(3, temp.Length - 3);
            }

            wordCount = convert.HexToDecimal(temp.Substring(0, 2));
            temp = temp.Substring(3, temp.Length - 3); //The temp string now contains only the priority number
            priority = convert.HexToDecimal(temp);
            ramIterator = priority - 1; //Index where PCB will be inserted - will save time on sorting
            while (Object.ReferenceEquals(this.queue[ramIterator], null) == false)
            {
                ramIterator++;
            }

            this.queue[ramIterator] = new PCB();
            this.queue[ramIterator].processID = pID; //Assign pID, priority number and wordCount to the PCB
            this.queue[ramIterator].priority = priority;
            this.queue[ramIterator].wordCount = wordCount;
            this.queue[ramIterator].diskAddress = diskPointer;
            this.queue[ramIterator].getPageCount();
            this.queue[ramIterator].pageTable = new page[this.queue[ramIterator].pageCount];
            this.queue[ramIterator].initializePageTable();

        }

        public void setDataInfo(String str)
        {
            this.queue[ramIterator].inputBuffer = convert.HexToDecimal(str.Substring(0, 2));
            this.queue[ramIterator].outputBuffer = convert.HexToDecimal(str.Substring(3, 1));
            this.queue[ramIterator].tempBuffer = convert.HexToDecimal(str.Substring(5, 1));
            this.queue[ramIterator].spaceReq = (this.queue[ramIterator].wordCount + this.queue[ramIterator].inputBuffer + this.queue[ramIterator].outputBuffer + this.queue[ramIterator].tempBuffer) * 32;


        }

        public void sortQueue() //Makes the queue a priority queue
        {
            for (int i = 0; i < this.queue.Length - 1; i++)
            {
                for (int j = i + 1; j < this.queue.Length; j++)
                {
                    if (this.queue[i].priority > this.queue[j].priority)
                    {
                        PCB temp = new PCB();
                        temp = this.queue[i];
                        this.queue[i] = this.queue[j];
                        this.queue[j] = temp;
                    }
                }
            }
        }

        public void showInfo()
        {
            for (int i = 0; i < this.size; i++)
            {
                queue[i].showInfo();
                Console.WriteLine("END");
            }
        }

        public void calculateTimes()
        {
            TimeSpan t1 = DateTime.Now.TimeOfDay;
            TimeSpan t2 = DateTime.Now.TimeOfDay;
            t1 = t1 - t1;
            t2 = t2 - t2;
            for (int i = 0; i < this.size; i++)
            {
                t1 = t1 + this.queue[i].waitTime;
                t2 = t2 + this.queue[i].executionTime;
                totalExecution += queue[i].executionTime;
            }
            this.avgWait = t1 / this.size;
            this.avgExecution = t2 / this.size;
        }
        public void showMetrics()
        {
            Console.WriteLine("\n\n********************METRICS********************\n\n");
            for (int i = 0; i < this.size; i++)
            {
                queue[i].showMetrics();
                Console.WriteLine("END");
            }
            Console.WriteLine("Average wait time: " + this.avgWait.Milliseconds + "ms");
            Console.WriteLine("Average execution time: " + this.avgExecution + " seconds");
            Console.WriteLine("Total execution time: " + this.totalExecution + " seconds");
        }

        public void printTime()
        {
            Console.WriteLine("Average wait time: " + this.avgWait.Milliseconds + "ms");
            Console.WriteLine("Average execution time: " + this.avgExecution + " seconds");
            Console.WriteLine("Total execution time: " + this.totalExecution + " seconds");
        }

        public Boolean jobsComplete() //A function for testing if all jobs have been completed
        {
            for (int i = 0; i < queue.Length; i++)
            {
                if (queue[i].complete == false)
                {
                    return false;
                }
            }
            return true;
        }

        public string getCoreDump(PCB pcb, RAM ram)
        {
            var dumpString = ram.coreDump(pcb.ramBaseAddress, pcb.ramBaseAddress + (pcb.wordCount + pcb.inputBuffer + pcb.outputBuffer + pcb.tempBuffer) * 32);
            return dumpString;
        }

        public void displayCoreDump()
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = folder + "/coreDump.txt";
            File.Delete(filepath);
            for (int i = 0; i < queue.Length; i++)
            {
                queue[i].displayCoreDump();
            }
        }

        public void getPageInfo()
        {
            for (int i = 0; i < queue.Length; i++)
            {
                queue[i].getPageInfo();
            }
        }

        public void showPageTable()
        {
            for (int i = 0; i < queue.Length; i++)
            {
                queue[i].showPageTable();
            }
        }



    }
}
