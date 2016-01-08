using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateRedirectionRulesFile();
            Server server = new Server(1000, "redirectionRules.txt");
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            FileStream file = new FileStream("redirectionRules.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine("aboutus.html,aboutus2.html");
            writer.Close();
        }

    }
}
