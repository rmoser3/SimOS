using System;
using System.Collections.Generic;
using System.Text;

namespace SimOS
{
    class page
    {
		public Boolean valid; //If true, the page is in memory. If false, it is not.
		public int diskBaseAddress; //The starting disk address of the page
		public int ramBaseAddress; //The starting ram address of the page
		public int wordCount; //The number of words within the page
		public int frameNumber; //which frame the page is allocated to 
		public page()
		{
			valid = false;
			diskBaseAddress = -1;
			ramBaseAddress = -1;
			wordCount = 0;
			frameNumber = -1;
		}

	}
}

