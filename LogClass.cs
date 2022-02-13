using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    // Singleton class for Log object
    internal class LogClass
    {
        private static readonly LogClass _log = new LogClass();
        private static String _filePath = "C:/Users/Liora/source/repos/ConsoleApp3/ConsoleApp3/Logs.txt";
        private LogClass()
        {
        }

        internal static LogClass GetLogger()
        {
            return _log;
        }

        internal void WriteLog(String logMessage)
        {
            try
            {
                // Write log message to log file
                using (FileStream fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter objStreamWriter = new StreamWriter((Stream)fileStream);
                    objStreamWriter.WriteLine(logMessage);
                    Console.WriteLine(logMessage);
                    objStreamWriter.Close();
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
               // return false;
            }
        }
    }
}
