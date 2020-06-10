using System;
using System.IO;

namespace BookLogger
{

    public class Logfile
    {

        StreamWriter file; //log file stream

        public Logfile()
        {
			//Constructor makes empty text file in current directory

            //Initialise empty logfile in current directory
            string pwd = Directory.GetCurrentDirectory();
            FileStream emptyFile = new FileStream(pwd + "/log.txt", FileMode.Create);
	        file = new StreamWriter(emptyFile);
            file.AutoFlush = true;

            //Header
            file.WriteLine("Program begun at: {0}",DateTime.Now.ToString());
            WriteDashes();
        }

        public void WriteDashes(int n = 60)
        { 
			//Write dashes
			
			string dashes="";
            for (int i = 0; i <= n; ++i) dashes += "-";
            file.WriteLine(dashes);
		}

        public void WriteLine<T>(T text)
        {
			//Write to file

            file.WriteLine(text);
		}

		public void WriteLine<S,T>(S text1, T text2)
        {
			//Write to file

            file.WriteLine("{0} {1}",text1,text2);
		}

    }
}
