using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

/*
 * Programmer           : Colby Taylor 8466914
 * Project              : MyOwnWebServer A06
 * File                 : Listener.cs
 * Class                : PROG2001 Web Design and Development
 * Date                 : 11/27/2021
 * Description          : this class takes the webRoot, webIP, webPort
 *                      : and starts a perpetual loop listeneing for
 *                      : TCPIP clients, and once a client connects
 *                      : the network stream is decoded and sent to 
 *                      : the class HandleRequest
 * 
 */



namespace MyOwnWebServer
{
    class Listener
    {
        public volatile bool Run = true;
        int BUFFERSIZE = 1024;
        TcpListener server = null;


        /*
         * function     : Listener()
         * Parameters   : string webRoot - holds the root of the web server
         *              : string webIP - holds the ip address for use with the TCPListener
         *              : string webPort - holds the port number for use with TCPListener
         * Return       : void
         * Description  : creates a perpetual loop to listen for incoming tcpip lcients requesting
         *              : webpages 
         */
        public void Listen(string webRoot, string webIP, string webPort)
        {
            try
            {
                FileStream file = File.Create("MyOwnWebServer.Log");
                file.Close();
                Logger.Log("[SERVER STARTED] using Web Root:" + webRoot + " Ip Address:" + webIP + " Port:" + webPort);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                int port = Int32.Parse(webPort); 
                IPAddress ipAddress = IPAddress.Parse(webIP);
                TcpListener server = new TcpListener(ipAddress, port);
                server.Start();
                byte[] bytes = new byte[16*BUFFERSIZE];
                string request = null;
                HandleRequest handle = new HandleRequest();
                while (Run)
                {
                    try
                    {
                        if (server.Pending())// if there is a client waiting then enter
                        {
                            TcpClient client = server.AcceptTcpClient();
                            NetworkStream stream = client.GetStream();
                            int i = stream.Read(bytes, 0, bytes.Length);
                            request = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            byte[] msg = handle.process(request, webRoot);
                            
                            if(msg == null)
                            {
                                msg = System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 404 error page not found");
                            }
                            stream.Write(msg, 0, msg.Length);
                            client.Close();
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    catch(Exception ex)// catch and log any exceptions from the TCPIP
                    {
                        Logger.Log(ex.Message);
                    }
                    
                }
            }
            catch(Exception ex)//catch any catastrophic errors ending the listner
            {
                Logger.Log(ex.Message);
            }
            finally
            {
                if(server != null)
                {
                    Logger.Log("[SERVER STOPPED]");
                    server.Stop();
                }
            }
        }
    }
}
