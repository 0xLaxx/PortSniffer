using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webbrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter name of server...");
            string server = Console.ReadLine();

            TcpClient client = new TcpClient(server, 80);
            StreamReader sr = new StreamReader(client.GetStream());
            StreamWriter sw = new StreamWriter(client.GetStream());

            try
            {
                sw.WriteLine("GET / HTTP/1.0\n\n");
                sw.Flush();

                string data = sr.ReadLine();
                while (data != null)
                {
                    Console.WriteLine(data);
                    data = sr.ReadLine();
                }
            }
            catch (Exception)
            {

            }

            Console.ReadLine();
        }
    }
}
