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
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {          
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString());
            if (redirectoinPath != string.Empty)
                headerLines.Add(redirectoinPath);

            // TODO: Create the request string
            responseString = GetStatusLine(code)+"\r\n"+contentType+"\r\n\r\n"+content;
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
