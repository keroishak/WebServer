using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            Configuration.RedirectionRules = new Dictionary<string, string>();
            this.LoadRedirectionRules(redirectionMatrixPath);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            serverSocket.Listen(500);
            Socket clientSocket;
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                clientSocket = serverSocket.Accept();
                //HandleConnection(clientSocket);
                Thread thr = new Thread(new ParameterizedThreadStart(target => HandleConnection(clientSocket)));
                thr.Start();
            }
        }

        public void HandleConnection(Socket ClientSocket)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            ClientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request                   
                    // TODO: break the while loop if receivedLen==0                    
                    // TODO: Create a Request object using received request string                    
                    // TODO: Call HandleRequest Method that returns the response                    
                    // TODO: Send Response back to client
                    byte[] arr = new byte[1024];
                    int len = ClientSocket.Receive(arr);
                    if (len == 0)
                        break;
                    Request request = new Request(ASCIIEncoding.ASCII.GetString(arr, 0, len));
                    Response respose = HandleRequest(request);
                    byte[] arr1 = ASCIIEncoding.ASCII.GetBytes(respose.ResponseString);
                    ClientSocket.Send(arr1);
                    // TODO: close client socket
                    ClientSocket.Close();
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
                
            }

           
        }

        Response HandleRequest(Request request)
        {
            string content;
            try
            {
                //TODO: check for bad request                
                //TODO: map the relativeURI in request to get the physical path of the resource.
                //TODO: check for redirect
                //TODO: check file exists
                //TODO: read the physical file
                // Create OK response
                if (request.ParseRequest() == false)
                    return new Response(StatusCode.BadRequest, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                string PhysicalPath ;
                string RedirectedPath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (RedirectedPath == string.Empty)
                    PhysicalPath = Configuration.RootPath + "\\" + request.relativeURI;
                else
                    PhysicalPath = Configuration.RootPath + "\\" + RedirectedPath;      
                if(!File.Exists(PhysicalPath))
                    return new Response(StatusCode.NotFound, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.NotFoundDefaultPageName), RedirectedPath);
                FileStream file = new FileStream(PhysicalPath, FileMode.Open);
               StreamReader reader=new StreamReader(file);
               content = reader.ReadToEnd();
                
               
                return new Response(StatusCode.OK, "text/html; charset=UTF-8", content, RedirectedPath);

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                return new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            return string.Empty;
        }
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string

            // else read file and return its content
            if (File.Exists(filePath))
            {
                FileStream file = new FileStream(filePath, FileMode.Open);

                StreamReader FileReader = new StreamReader(file);

                /*byte[] arrofFilelength = BitConverter.GetBytes(file.Name.Length);
                byte[] arrofFileName = Encoding.ASCII.GetBytes(file.Name);
                byte[] FileContent = Encoding.ASCII.GetBytes(FileReader.ReadToEnd());
                byte[] FileInfo = new byte[file.Length + file.Name.Length + 4];
                arrofFilelength.CopyTo(FileInfo, 0);
                arrofFileName.CopyTo(FileInfo, 4);
                FileContent.CopyTo(FileInfo, 4 + arrofFileName.Length);*/
                return FileReader.ReadToEnd();
            }
            Logger.LogException(new Exception(defaultPageName + " file not found"));
            return string.Empty;
        }
        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                FileStream file = new FileStream(filePath, FileMode.OpenOrCreate);
                StreamReader reader = new StreamReader(file);
                string s;
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (!reader.EndOfStream)
                {
                    s = reader.ReadLine();
                    string[] arr = s.Split(',');
                    Configuration.RedirectionRules.Add(arr[0], arr[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
