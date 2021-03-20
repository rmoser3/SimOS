using System;
using System.Collections.Generic;
using System.Text;

namespace SimOS
{
    class cpuQueue //The purpose of this class is to ensure that only the CPU at the head of the queue may access the pcbQueue, to prevent race conditions. 
    {
        public CPU head;

        public void enQueue(CPU cpu)
        {
            if(head == null)
            {
                head = cpu;
            }
            else
            {
                CPU temp = head;
                while(temp.next != null)
                {
                    temp = temp.next;
                }
                temp.next = cpu;
            }
        }
        public void deQueue()
        {
            CPU temp = head;
            head = head.next;
            temp.next = null;
        }

        public void display()
        {
            Console.WriteLine("Queue status:");
            Console.WriteLine("Queue head is: " + head.id);
            CPU temp = head;
            while(temp != null)
            {
                Console.WriteLine(temp.id);
                temp = temp.next;
            }
        }
    }
}
