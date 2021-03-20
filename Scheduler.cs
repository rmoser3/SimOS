using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SimOS
{
    class Scheduler
    {
        int iterator = 0; //This points at a specific index in the RAM's data array
        Disk disk;
        RAM ram;
        pcbQueue pcbQueue;
        List<string> instructionList;
        bool debugMode = false;

        public Scheduler()
        {

        }

        public Scheduler(bool debugMode)
        {
            this.debugMode = debugMode;
        }

        public void pageLoad(Disk disk, RAM ram, pcbQueue pcbQueue)
        {
            //for each pcb load the first 4 pages in RAM
            while(pcbQueue.ramIterator < 30)
            {
                for(int i = 0; i < 4; i++)
                {
                    pageLoadRam(disk, pcbQueue.queue[pcbQueue.ramIterator].pageTable[i], ram);
                }
                pcbQueue.ramIterator++;
            }
        }

        public void requestPageLoad(Disk disk, RAM ram, page page)
        {
            pageLoadRam(disk, page, ram);
        }

        public void load(Disk disk, RAM ram, pcbQueue pcbQueue) //Loads the next batch of jobs into RAM 
        {

            while (pcbQueue.jobsComplete() == false && pcbQueue.ramIterator < 30)
            {
                int index = pcbQueue.ramIterator; //Starting load index is where the ramIterator currently points 
                while (spaceAvailable(iterator, pcbQueue.queue[index].spaceReq, ram) == false)
                {
                    //do nothing
                    Thread.Yield();
                }
                while (spaceAvailable(iterator, pcbQueue.queue[index].spaceReq, ram) == true) //It will stop loading when space is no longer available for next process
                {
                    if (pcbQueue.ramIterator >= pcbQueue.queue.Length)
                    {
                        break;
                    }
                    loadRAM(disk, ram, pcbQueue, pcbQueue.ramIterator);
                }
                ram.calculateUsed();
                if (debugMode)
                    Console.WriteLine("Percentage of RAM used: " + ram.percentageUsed + "%");
                iterator = 0;
            }


        }

        public void pageLoadRam(Disk disk, page page, RAM ram)
        {
            int index = ram.getFreeFrame();
            if(index == ram.frameList.Length)
            {
                return; //no available memory
            }
            page.frameNumber = index;
            page.ramBaseAddress = ram.frameList[index].startAddress;
            ram.frameList[index].available = false;
            int ramPointer = ram.frameList[index].startAddress;
            int diskPointer = page.diskBaseAddress;
            for(int i = 0; i < 128; i++)
            {
                ram.data[ramPointer] = disk.data[diskPointer];
                ramPointer++;
                diskPointer++;
            }
            page.valid = true;
        }

        public void loadRAM(Disk disk, RAM ram, pcbQueue pcbQueue, int i)
        {
            //Load an individual job from disk to RAM, starting at process disk address and ending at disk address + spaceReq
            pcbQueue.queue[i].ramBaseAddress = iterator;
            pcbQueue.queue[i].programCounter = iterator;
            pcbQueue.queue[i].ramLoadTime = DateTime.Now.TimeOfDay;
            pcbQueue.queue[i].inputBufferAddress = pcbQueue.queue[i].ramBaseAddress + pcbQueue.queue[i].wordCount * 32 + 1;
            pcbQueue.queue[i].outputBufferAddress = pcbQueue.queue[i].inputBufferAddress + pcbQueue.queue[i].inputBuffer * 32 + 1;
            pcbQueue.queue[i].tempBufferAddress = pcbQueue.queue[i].outputBufferAddress + pcbQueue.queue[i].outputBuffer * 32 + 1;
            for (int j = pcbQueue.queue[i].diskAddress; j <= pcbQueue.queue[i].diskAddress + pcbQueue.queue[i].spaceReq; j++)
            {
                ram.data[iterator] = disk.data[j];
                iterator++;
            }
            pcbQueue.ramIterator++; //By the end of this function, the queue's iterator will points to next process to be loaded
        }

        public Boolean spaceAvailable(int iterator, int spaceReq, RAM ram) //Tells us if space is available starting at position [iterator] up to [iterator+spaceReq]
        {
            Boolean result = true;
            for (int i = iterator; i <= iterator + spaceReq; i++)
            {
                if (i == 32768 || i < 0) //If we reach the last index of RAM
                {
                    return result = false;
                }
                if (ram.data[i] != -1)
                {
                    return result = false;
                }
            }
            return result;
        }

        public void releaseMem(RAM ram, PCB pcb) //Releases an individual processes memory 
        {
            for (int i = pcb.ramBaseAddress; i <= pcb.ramBaseAddress + pcb.spaceReq; i++)
            {
                if (i >= ram.data.Length || i < 0)
                {
                    if (debugMode)
                        Console.WriteLine("ERROR! i is " + i);
                }
                ram.data[i] = -1;
            }
        }

        public void releasePage(page page, RAM ram)
        {
            page.valid = false;
            ram.frameList[page.frameNumber].available = true;
            for(int i = page.ramBaseAddress; i < page.ramBaseAddress + 128; i++)
            {
                ram.data[i] = -1;
            }
        }

        public void releaseAllMem(RAM ram) //Wipes RAM
        {
            for (int i = 0; i < ram.data.Length; i++)
            {
                ram.data[i] = -1;
            }
        }
    }
}
