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
            Configuration.RedirectionRules = new Dictionary<string, string>();
            this.LoadRedirectionRules(redirectionMatrixPath);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
        }

        public void StartServer()
        {
            serverSocket.Listen(500);
            Socket clientSocket;
            while (true)
            {
                clientSocket = serverSocket.Accept();
                Thread thr = new Thread(new ParameterizedThreadStart(target => HandleConnection(clientSocket)));
                thr.Start();
            }
        }

        public void HandleConnection(Socket ClientSocket)
        {
            ClientSocket.ReceiveTimeout = 0;
            //receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    byte[] arr = new byte[2 * 1024];
                    int len = ClientSocket.Receive(arr);
                    if (len == 0)
                        break;
                    Request request = new Request(ASCIIEncoding.ASCII.GetString(arr, 0, len));
                    Response respose = HandleRequest(request);
                    byte[] arr1 = ASCIIEncoding.ASCII.GetBytes(respose.ResponseString);
                    ClientSocket.Send(arr1);
                    ClientSocket.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }

            }


        }

        Response HandleRequest(Request request)
        {
            string content;
            try
            {

                if (request.ParseRequest() == false)
                    return new Response(StatusCode.BadRequest, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");

                string RedirectedPath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (RedirectedPath != string.Empty)
                    return new Response(StatusCode.Redirect, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.RedirectionDefaultPageName), "");
                string PhysicalPath = Configuration.RootPath + "\\" + request.relativeURI;

                //PhysicalPath = Configuration.RootPath + "\\" + RedirectedPath;      
                if (!File.Exists(PhysicalPath))
                    return new Response(StatusCode.NotFound, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.NotFoundDefaultPageName), RedirectedPath);
                FileStream file = new FileStream(PhysicalPath, FileMode.Open);
                StreamReader reader = new StreamReader(file);
                content = reader.ReadToEnd();


                return new Response(StatusCode.OK, "text/html; charset=UTF-8", content, RedirectedPath);

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            return string.Empty;
        }
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            if (File.Exists(filePath))
            {
                FileStream file = new FileStream(filePath, FileMode.Open);

                StreamReader FileReader = new StreamReader(file);

                return FileReader.ReadToEnd();
            }
            Logger.LogException(new Exception(defaultPageName + " file not found"));
            return string.Empty;
        }
        private void LoadRedirectionRules(string filePath)
        {
            try
            {
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
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
