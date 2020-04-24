using DataAccessLibrary;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PortSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
            Console.Read();
        }

        private static void Start()
        {
            TcpListener server = null;
            
            //todo - get values from user.config
            IPAddress ip = IPAddress.Loopback;
            int port = 55779;
            string table = "ServerDb";
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

            try
            {
                server = new TcpListener(ip,port);
                server.Start();

                byte[] bytes = new byte[256];
                string jsonData = null;

                while (true)
                {
                    Console.WriteLine("Waiting for Connection...");
                    TcpClient client = server.AcceptTcpClient();
                    
                    Console.WriteLine("Connected");

                    jsonData = null;
                    NetworkStream networkStream = client.GetStream();

                    int i;
                    while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        jsonData += Encoding.ASCII.GetString(bytes, 0, i);
                    }

                    Console.WriteLine($"Received: {jsonData}");

                    var db = new ServerDatabaseConnection(table, ip.ToString(), port, connectionString);
                    
                    //todo - analyse if valid json
                    db.InsertJsonToDb(jsonData);

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
