using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimOS
{
    class Loader
    {
        Disk disk;
        string filename;
        public pcbQueue pcbQueue;
        //Default constructor, set to search for filename "program-file" 
        public Loader(Disk disk)
        {
            this.disk = disk;
            this.filename = "program-file.txt";
        }

        //Alterantive constructor, lets user choose filename to load from 
        public Loader(Disk disk, string filename)
        {
            this.disk = disk;
            this.filename = filename;
        }

        //Function loads data into disk 
        public void Load()
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            int counter = 0;
            pcbQueue = new pcbQueue(30);

            while (counter < lines.Length)
            {

                if (lines[counter].Contains("//") == false)
                {
                    disk.enterData(lines[counter]);
                }
                else
                {
                    if (lines[counter].Contains("JOB") == true)
                    {
                        //Get job number, priority number, number of words and disk pointer for  store them in PCB's in queue
                        pcbQueue.setJobInfo(lines[counter].Substring(7, lines[counter].Length - 7), disk.pointer * 32);
                    }
                    else if (lines[counter].Contains("Data") == true)
                    {
                        //Get input, output and temporary buffers, store them in PCB's in queue
                        pcbQueue.setDataInfo(lines[counter].Substring(8, lines[counter].Length - 8));
                    }
                }
                counter++;
            }
            pcbQueue.ramIterator = 0;
            pcbQueue.sortQueue();
        }
    }
}