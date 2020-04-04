using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PortSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            //TcpDemo();

            

            Console.Read();
        }

        private static void TcpDemo()
        {
            TcpListener server = null;
            int port = 55779;
            IPAddress ip = IPAddress.Loopback;

            try
            {
                server = new TcpListener(port);
                server.Start();

                byte[] bytes = new byte[256];
                string data = null;

                while (true)
                {
                    Console.WriteLine("Waiting for Connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected");

                    data = null;

                    NetworkStream networkStream = client.GetStream();

                    int i;

                    while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine($"Received: {data}");

                        data = data.ToUpper();
                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        networkStream.Write(msg, 0, msg.Length);
                        Console.WriteLine($"Sent: {data}");
                    }

                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
