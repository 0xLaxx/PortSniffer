using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(1302);
            listener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for connection...");

                TcpClient client = listener.AcceptTcpClient();
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());

                try
                {
                    //client request
                    string request = sr.ReadLine();
                    Console.WriteLine(request);

                    string[] tokens = request.Split(' ');
                    string page = tokens[1];

                    if (page == "/")
                    {
                        page = "/test.html";
                    }

                    StreamReader file = new StreamReader("../../web" + page);
                    sw.WriteLine("HTTP/1.0 200 OK\n");

                    //send the file
                    string data = file.ReadLine();
                    while (data != null)
                    {
                        sw.WriteLine(data);
                        sw.Flush();
                        data = file.ReadLine();
                    }


                }
                catch (Exception)
                {
                    sw.WriteLine("HTTP/1.0 404 OK\n");
                    sw.WriteLine("<H1>SORRY! We couldnt fin your file</H1>");
                    sw.Flush();
                }
                finally
                {
                    client.Close();
                }
            }
        }
    }
}
