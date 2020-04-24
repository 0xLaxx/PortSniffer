using DataAccessLibrary;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PortSniffer
{
    class Program
    {
        #region Variables
        public static Action<string, ConsoleColor> ColoredPrint
            = new Action<string, ConsoleColor>((message, color) =>
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            });

        #endregion
     
        #region Main
        static void Main(string[] args)
        {
            //Todo - put inside windows service
            Start();
            Console.Read();
        }

        #endregion

        #region Server logic
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
                server = new TcpListener(ip, port);
                server.Start();

                byte[] bytes = new byte[256];
                string jsonData = null;

                while (true)
                {
                    ColoredPrint("Waiting for Connection...", ConsoleColor.Yellow);
                    TcpClient client = server.AcceptTcpClient();
                    ColoredPrint("Connected", ConsoleColor.Green);

                    jsonData = null;
                    NetworkStream networkStream = client.GetStream();

                    int i;
                    while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        jsonData += Encoding.ASCII.GetString(bytes, 0, i);
                    }

                    ColoredPrint("Successfully received data. Printing data...", ConsoleColor.Yellow);
                    Console.WriteLine(jsonData);

                    var db = new ServerDatabaseConnection(table, ip.ToString(), port, connectionString);

                    //events
                    db.AlterTableEvent += Db_Event;
                    db.InsertEvent += Db_Event;
                    db.ErrorEvent += Db_Error;

                    //insert
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

        #endregion

        #region Event methods
        private static void Db_Event(object sender, DbEventArgs e)
        {
            ColoredPrint(e.Message, ConsoleColor.Yellow);
        }

        private static void Db_Error(object sender, DbEventArgs e)
        {
            ColoredPrint(e.Message, ConsoleColor.Red);
        } 
        #endregion
    }
}
