using System;
using System.Collections.Generic;
using System.Text;

namespace SimOS
{
    public class frame
    {
        public int startAddress;
        public Boolean available;
        public frame()
        {
            startAddress = 0;
            available = true;
        }
        public frame(int address)
        {
            this.startAddress = address;
            this.available = true;
        }
    }
}
