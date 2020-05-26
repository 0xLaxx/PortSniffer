using DataAccessLibrary;
using LogLibrary;
using SettingsLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PortSniffer
{
    public class Server
    {
        #region Fields

        //Serversettings
        TcpListener server;
        IPAddress ip = Settings.Get<IPAddress>(nameof(SettingsProperties.IP));
        int port = Settings.Get<int>(nameof(SettingsProperties.Port));

        //Databse settings
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

                Logger.LogEvent($"Server started on {ip}:{port}");
                Logger.LogMessage($"Table {table} on {connectionString} selected. Saving to database {(saveToDb ? "enabled" : "disabled")}.");

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);

                Stop();
                return false;
            }
        }
        public void Stop()
        {
            Logger.LogEvent("Server stopped");
            server.Stop();
        }

        public void Listen()
        {
            byte[] bytes = new byte[256];
            string jsonData = null;

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                jsonData = null;
                NetworkStream networkStream = client.GetStream();

                int i;
                while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    jsonData += Encoding.ASCII.GetString(bytes, 0, i);
                }

                Logger.LogEvent($"Successfully received data of length {jsonData.Length}.");

                var db = new ServerDatabaseConnection(table, ip.ToString(), port, connectionString);

                //insert
                if (saveToDb)
                    db.InsertJsonToDb(jsonData);


                client.Close();
            }
        }

        #endregion
    }
}
