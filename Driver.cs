using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SimOS
{
    class Driver
    {
        bool debugMode = false;
        Disk disk;
        RAM ram;
        Loader load;
        Scheduler schedule;
        CPU cpu1, cpu2, cpu3, cpu4;

        public Driver(bool debugMode)
        {
            this.debugMode = debugMode;
            schedule = new Scheduler(debugMode);
            cpu1 = new CPU(debugMode);
            cpu2 = new CPU(debugMode);
            cpu3 = new CPU(debugMode);
            cpu4 = new CPU(debugMode);
            disk = new Disk();
            ram = new RAM();
            load = new Loader(disk);
        }

        public Driver(bool debugMode, string filename)
        {
            this.debugMode = debugMode;
            schedule = new Scheduler(debugMode);
            cpu1 = new CPU(debugMode);
            cpu2 = new CPU(debugMode);
            cpu3 = new CPU(debugMode);
            cpu4 = new CPU(debugMode);
            disk = new Disk();
            ram = new RAM();
            load = new Loader(disk, filename);
        }

        public void run_1()
        {
            this.setID();
            cpuQueue que = new cpuQueue();
            que.enQueue(cpu1);
            load.Load();
            Thread loadThread = new Thread(() => schedule.load(disk, ram, load.pcbQueue));
            Thread cpuThread1 = new Thread(() => cpu1.execute(ram, load.pcbQueue, schedule, que));

            loadThread.Start();
            cpuThread1.Start();

            while (load.pcbQueue.jobsComplete() == false)
            {
                //do nothing
            }
            if (load.pcbQueue.jobsComplete() == true)
            {
                if (debugMode)
                    load.pcbQueue.displayCoreDump();
                load.pcbQueue.calculateTimes();
                Console.WriteLine("******************************* 1 CPU ***********************************");
                if (debugMode)
                    load.pcbQueue.showMetrics();
                else
                    load.pcbQueue.printTime();
            }
        }

        public void run_4()
        {
            this.setID();
            cpuQueue que = new cpuQueue();
            que.enQueue(cpu1);
            que.enQueue(cpu2);
            que.enQueue(cpu3);
            que.enQueue(cpu4);
            load.Load();
            Thread loadThread = new Thread(() => schedule.load(disk, ram, load.pcbQueue));
            Thread cpuThread1 = new Thread(() => cpu1.execute(ram, load.pcbQueue, schedule, que));
            Thread cpuThread2 = new Thread(() => cpu2.execute(ram, load.pcbQueue, schedule, que));
            Thread cpuThread3 = new Thread(() => cpu3.execute(ram, load.pcbQueue, schedule, que));
            Thread cpuThread4 = new Thread(() => cpu4.execute(ram, load.pcbQueue, schedule, que));

            loadThread.Start();
            cpuThread1.Start();
            cpuThread2.Start();
            cpuThread3.Start();
            cpuThread4.Start();

            while (load.pcbQueue.jobsComplete() == false)
            {
                //do nothing
            }
            if (load.pcbQueue.jobsComplete() == true)
            {
                if (debugMode)
                    load.pcbQueue.displayCoreDump();
                load.pcbQueue.calculateTimes();
                Console.WriteLine("******************************* 4 CPU ***********************************");
                if (debugMode)
                    load.pcbQueue.showMetrics();
                else
                    load.pcbQueue.printTime();
                ram.getUsed();
            }
        }

        public void page_run_4()
        {
            this.setID();
            cpuQueue que = new cpuQueue();
            que.enQueue(cpu1);
            que.enQueue(cpu2);
            que.enQueue(cpu3);
            que.enQueue(cpu4);
            load.Load();
            load.pcbQueue.getPageInfo();
            schedule.pageLoad(disk, ram, load.pcbQueue);
            ram.display();
            load.pcbQueue.showPageTable();

            while (load.pcbQueue.jobsComplete() == false)
            {
                //do nothing
            }
            if (load.pcbQueue.jobsComplete() == true)
            {
                load.pcbQueue.displayCoreDump();
                load.pcbQueue.calculateTimes();
                Console.WriteLine("******************************* 4 CPU ***********************************");
                if (debugMode)
                    load.pcbQueue.showMetrics();
                else
                    load.pcbQueue.printTime();
                //ram.showFrameList();


                //ram.getUsed();
            }
        }


        public void setID()
        {
            cpu1.id = "1";
            cpu2.id = "2";
            cpu3.id = "3";
            cpu4.id = "4";
        }

    }
}