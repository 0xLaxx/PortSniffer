using DataAccessLibrary;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PortSniffer
{
    public class Server
    {
        #region Variables
        Action<string, ConsoleColor> ColoredPrint
            = new Action<string, ConsoleColor>((message, color) =>
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            });

        #endregion

        #region Fields
        TcpListener server;
        IPAddress ip = Settings.Get<IPAddress>(nameof(SettingsProperties.IP));
        int port = Settings.Get<int>(nameof(SettingsProperties.Port));
        string table = Settings.Get<string>(nameof(SettingsProperties.Table));
        string connectionString = Settings.Get<string>(nameof(SettingsProperties.ConnectionString));
        bool saveToDb = Settings.Get<bool>(nameof(SettingsProperties.SaveToDatabase));
        #endregion

        public Server()
        {
            server = new TcpListener(ip, port);
        }

        #region Server logic
        public bool Start()
        {
            try
            {
                server.Start();
                var listenerThread = new Thread(new ThreadStart(Listen));
                listenerThread.IsBackground = true;
                listenerThread.Start();
                return true;
            }
            catch (Exception) //todo log
            {
                //ColoredPrint(e.Message, ConsoleColor.Red);

                Stop();
                return false;
            }
        }
        public void Stop()
        {
            server.Stop();
        }

        public void Listen()
        {
            byte[] bytes = new byte[256];
            string jsonData = null;

            while (true)
            {
                //ColoredPrint("\nWaiting for Connection...", ConsoleColor.Yellow);
                TcpClient client = server.AcceptTcpClient();
                //ColoredPrint("\nConnected", ConsoleColor.Green);

                jsonData = null;
                NetworkStream networkStream = client.GetStream();

                int i;
                while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    jsonData += Encoding.ASCII.GetString(bytes, 0, i);
                }

                //ColoredPrint("\nSuccessfully received data. Printing data...", ConsoleColor.Yellow);
                //Console.WriteLine(jsonData + "\n");

                var db = new ServerDatabaseConnection(table, ip.ToString(), port, connectionString);

                //events
                //db.AlterTableEvent += Db_Alter;
                //db.InsertEvent += Db_Insert;
                //db.ErrorEvent += Db_Error;

                //insert
                if (saveToDb)
                    db.InsertJsonToDb(jsonData);


                client.Close();
            }
        }

        #endregion

        #region Event methods
        //void Db_Alter(object sender, DbEventArgs e)
        //{
        //    ColoredPrint(e.Message, ConsoleColor.Yellow);
        //}

        //void Db_Insert(object sender, DbEventArgs e)
        //{
        //    ColoredPrint(e.Message, ConsoleColor.Green);
        //}

        //void Db_Error(object sender, DbEventArgs e)
        //{
        //    ColoredPrint(e.Message, ConsoleColor.Red);
        //}
        #endregion
    }
}
