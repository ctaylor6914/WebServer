using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*
 * Programmer           : Colby Taylor 8466914
 * Project              : MyOwnWebServer A06
 * File                 : Logger.cs
 * Class                : PROG2001 Web Design and Development
 * Date                 : 11/27/2021
 * Description          : this class takes a message and formats 
 *                      : the message to be entered into a log file
 * 
 */

namespace MyOwnWebServer
{
    class Logger
    {

        /*
         * function     : Log()
         * Parameters   : string logMessage - the message to be included in the logfile
         * Returns      : void
         * Description  : this fucntion takes a string message and adds it to 
         *              : the log message formula to create a nicle formatted log message in the 
         *              : root of where the .exe is running.
         *              : any errors are printed to the console. 
         */
        public static void Log(string logMessage)
        {
            try
            {
                string pathname = "myOwnWebServer.log";
                string logmsg = DateTime.Now.ToString() + ":" + logMessage;
                FileStream file;
                StreamWriter sw;
                file = File.Open(pathname, FileMode.Append);
                sw = new StreamWriter(file);
                sw.WriteLine(logmsg);
                sw.Close();
                file.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR" + ex.Message);
            }
            
        }
    }
}
