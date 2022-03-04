using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*
 * Programmer           : Colby Taylor 
 * File                 : HandleRequest.cs
 * Date                 : 11/27/2021
 * Description          : This class uses a method called process()
 *                      : which parses the incoming request string using
 *                      : the webRoot to search for the file
 * 
 */

namespace MyOwnWebServer
{
    class HandleRequest
    {

        /*
         * function     : process()
         * Parameters   : string message - the string value which holds the incoming HTTP message
         *              : string webRoot - a string value which holds the webRoot
         * Return       : byte[] for the HTTP response
         * Description  : this fucntion returns a byte[]  which will be sent back to the requesting 
         *              : HTTP client.  THis fucntion takes the incoming HTTP request, parses it and 
         *              : with the logic determines if the file exists, whcih type of file it is and   
         *              : encodes the header, message and returns it to the calling fucntion to be 
         *              : sent to the client
         */

        public byte[] process(string message, string webRoot)
        {
            string method    = null;
            string webPage   = null;
            string version   = null;
            string type      = null;
            string filePath  = null;
            byte[] msg       = null;
            try
            {
                string[] splitter = message.Split(' ');                 //  Parse the incoming string based on 
                method = splitter[0];                                   //  HTML/1.1 protocol
                webPage = splitter[1];                                  //  method is looking for "GET"
                webPage.Replace("/", "\\");                             //  webPage is looking for /requestedPage
                version = splitter[2];                                  //
                string[] getVersion = version.Split('\r');              //  version looking for HTTP/1.1
                version = getVersion[0];                                //  type is looking for the file extension
                string[] splitter2 = message.Split('.');                //
                type = splitter2[1];                                    //
                string[] getType = type.Split(' ');                     //
                type = getType[0].Trim();                               //
                filePath = webRoot + webPage;                           //
                
            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message);
            }
            

            if (method != "GET")    // if the method is not GET then erroe
            {
                message = version + " 400 incorrect verb";
                msg = System.Text.Encoding.ASCII.GetBytes(message);
                return msg;
            }
            if(version != "HTTP/1.1")       // if the version is not HTTP/1.1 then error
            {
                message = version + " 400 incorrect version";
                msg = System.Text.Encoding.ASCII.GetBytes(message);
                return msg;
            }

            type = type.ToLower();

            if(type == "asp" || type == "txt" || type == "html" || type == "php"|| type == "aspx") // if the file extension is an html format
            {
                string text = "text/";
                string holder = type;
                type = text + holder;
            }

            if(type == "jpg"|| type == "gif") // if the file extension is an image format
            {
                string text = "image/";
                string holder = type;
                type = text + holder;

                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    byte[] buf = new byte[fileInfo.Length];
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    fs.Read(buf, 0, buf.Length);
                    fs.Close();
                    GC.ReRegisterForFinalize(fileInfo);
                    GC.ReRegisterForFinalize(fs);

                    Logger.Log("[Request Received] Verb:" + method + " Resource Requested:" + webPage);


                    //construct message header
                    message = (version + " 200" + " OK\r\n" +
                        "Content-Type: " + type + "\r\n" +
                        "Content-Length: " + fileInfo.Length.ToString() + "\r\n" +
                        "Server: MyOwnServer\r\n" +
                        "Date:" + DateTime.Now.ToString() + "\r\n\r\n");

                    //construct response header
                    Logger.Log("[RESPONSE SENT] Content-Type: " + type + "Content-Length: " + 
                        fileInfo.Length.ToString()+ "Server: MyOwnServer" 
                        + "Date:" + DateTime.Now.ToString());

                    msg = System.Text.Encoding.ASCII.GetBytes(message); //encode message
                    byte[] ph = msg;
                    byte[] bytes = new byte[ph.Length + buf.Length]; //create a byte[] to hold header and image bytes
                    Buffer.BlockCopy(ph, 0, bytes, 0, ph.Length); //put the ph in the byte []
                    Buffer.BlockCopy(buf, 0, bytes, ph.Length, buf.Length);// put file bytes behind ph[] 
                    msg = bytes;
                }
                catch (DirectoryNotFoundException ex)
                {
                    message = "404 page not found " + ex.Message;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                }

                return msg;
            }

            try
            {
                //StreamReader reader = File.OpenText(filePath);
                StreamReader reader = new StreamReader(filePath);
            
                string line = "";
                StringBuilder stringBuilder = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    stringBuilder.Append(line);
                }
                
                Logger.Log("[Request Received] Verb:" + method + " Resource Requested:" + webPage);
                
                
                //construct response header
                message = (version + " 200" + " OK\r\n" +
                    "Content-Type: " + type + "\r\n" +
                    "Content-Length: " + stringBuilder.Length.ToString() + "\r\n" +
                    "Server: MyOwnServer\r\n" +
                    "Date:" + DateTime.Now.ToString() + "\r\n\r\n" +
                    stringBuilder);

                //construct logger message
                Logger.Log("[RESPONSE SENT] Content-Type: " + type + "Content-Length: " + 
                    stringBuilder.Length.ToString() + 
                    "Server: MyOwnServer" + "Date:" + DateTime.Now.ToString());
                msg = System.Text.Encoding.ASCII.GetBytes(message);
            }
            catch(DirectoryNotFoundException ex)
            {
                msg = System.Text.Encoding.ASCII.GetBytes("404 page not found " +ex.Message);
            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message);
            }

            return msg;
        }

    }
}
