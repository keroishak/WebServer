using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        //List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            string HeaderLines = "Server: " + Configuration.ServerType+"\r\n";
            HeaderLines += "Content-Type: " + contentType+"\r\n";
            HeaderLines += "Content-Length: " + content.Length.ToString() + "\r\n";
            HeaderLines += "Date: " + DateTime.Now.ToString() + "\r\n";
            if (redirectoinPath != string.Empty)
                HeaderLines+="Location: "+redirectoinPath+"\r\n";
            
            responseString = GetStatusLine(code)+"\r\n"+HeaderLines+"\r\n"+content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion + " ";
            if (code == StatusCode.BadRequest)
                statusLine += "400" + " BadRequest";
            else if (code == StatusCode.InternalServerError)
                statusLine += "500" + " InternalServerError";
            else if (code == StatusCode.NotFound)
                statusLine += "404" + " NotFound";
            else if (code == StatusCode.OK)
                statusLine += "200" + " OK";
            else if (code == StatusCode.Redirect)
                statusLine += "301" + " Redirect";
            return statusLine;
        }
    }
}
