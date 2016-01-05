using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            // for each exception write its details associated with datetime 
            FileStream file = new FileStream("log.txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(file);
            string row = DateTime.Now.ToLocalTime() + ex.Message;
            writer.WriteLine(row);
            writer.Close();
            file.Close();
        }
    }
}
