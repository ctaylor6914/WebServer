using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

/*
 * Programmer           : Colby Taylor 8466914
 * Project              : MyOwnWebServer A06
 * File                 : Program.cs
 * Class                : PROG2001 Web Design and Development
 * Date                 : 11/27/2021
 * Description          : The creation of a single threaded web server.
 *                      : once the server is started with a correct file
 *                      : as the web root and correct IP and port the code
 *                      : enters a perpetual loop listeneing for TCPIP clients
 *                      : requesting webpages, pictures or text
 * 
 */



namespace MyOwnWebServer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string webRoot = "";
            string webIP=null;
            string webPort = null;
            int portNum = 0;
            try
            {
                if(args.Length==0)//if there are no command line arguments
                {
                    Console.WriteLine("You didnt enter a command line argument, please enter a web Root");
                    webRoot = Console.ReadLine();
                    while (!Directory.Exists(webRoot))//check if webroot exists on machine
                    {
                        Console.WriteLine("Web Root does not exist.  Please enter a correct Web Root");
                        webRoot = Console.ReadLine();
                    }
                    Console.WriteLine("Please Enter an IP address");
                    webIP = Console.ReadLine();
                    Console.WriteLine("Please Enter a Port Number");
                    webPort = Console.ReadLine();

                    while (!Int32.TryParse(webPort, out portNum))// see if port number is infact a number
                    {
                        Console.WriteLine("Error Parsing Port. Please Enter a number for the Port");
                        webPort = Console.ReadLine();
                    }
                }
                else
                {
                    string[] spl = args[0].Split('=');
                    webRoot = spl[1];
                    while (!Directory.Exists(webRoot))//check if webroot exists on machine
                    {
                        Console.WriteLine("Web Root does not exist.  Please enter a correct Web Root");
                        webRoot = Console.ReadLine();
                    }
                    string[] spl1 = args[1].Split('=');
                    webIP = spl1[1];
                    string[] spl2 = args[2].Split('=');
                    webPort = spl2[1];


                    while (!Int32.TryParse(webPort, out portNum))// see if port number is infact a number
                    {
                        Console.WriteLine("Error Parsing Port. Please Enter a number for the Port");
                        webPort = Console.ReadLine();
                    }
                }
                
                
                Listener listener = new Listener();
                listener.Listen(webRoot, webIP, webPort);
                Console.WriteLine("Press any key to shut down");
                Console.ReadKey();
                listener.Run = false;
                Logger.Log("[SERVER SHUTDOWN]");
                Console.WriteLine("Server Shutting Down");
                Console.WriteLine("\nHit enter to continue...");
                Console.Read();
            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
    }
}
