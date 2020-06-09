using System;
using System.IO;

namespace BookLogger
{

    public class Logfile
    {

        StreamWriter file; //log file stream

        //Constructor makes empty text file in current directory
        public Logfile()
        {

            //Initialise empty logfile in current directory
            string pwd = Directory.GetCurrentDirectory();
            FileStream emptyFile = new FileStream(pwd + "/log.txt", FileMode.Create);
	        file = new StreamWriter(emptyFile);
            file.AutoFlush = true;

            //Header
            file.WriteLine("Program begun at: {0}",DateTime.Now.ToString());
            WriteDashes();
        }


        //Write dashes to screen
        public void WriteDashes(int n = 60)
        { 
			
			string dashes="";
            for (int i = 0; i <= n; ++i) dashes += "-";
            file.WriteLine(dashes);
		}

    }
}
