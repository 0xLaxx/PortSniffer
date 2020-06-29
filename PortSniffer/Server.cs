using DataAccessLibrary;
using LogLibrary;
using SettingsLibrary;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PortSniffer
{
    public class Server
    {
        #region Fields

        // Serversettings
        TcpListener server;
        IPAddress ip = Settings.Get<IPAddress>(nameof(SettingsProperties.IP));
        int port = Settings.Get<int>(nameof(SettingsProperties.Port)); 
         
        // XML settings for database
        string table = Settings.Get<string>(nameof(SettingsProperties.Table));
        string database = Settings.Get<string>(nameof(SettingsProperties.Database));
        string serverString = Settings.Get<string>(nameof(SettingsProperties.Server));
        bool saveToDb = Settings.Get<bool>(nameof(SettingsProperties.SaveToDatabase));
        string connectionString;

        #endregion

        public Server()
        {
            connectionString = BuildConnectionString();
            server = new TcpListener(ip, port);
        }

        //builds connection string from xml settings
        private string BuildConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = serverString;
            builder["Database"] = database;
            builder["Trusted_Connection"] = true;

            return builder.ConnectionString;
        }

        #region Server logic
        //start method for windows service - true if successful
        public bool Start()
        {
            try
            {
                //starts tcp listener
                server.Start(); 

                //new thread of listen method to avoid timeout in service
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
            //initialize variables 
            byte[] bytes = new byte[256];
            string jsonData = null;

            while (true)
            {
                //waits for data
                TcpClient client = server.AcceptTcpClient();

                jsonData = null;
                NetworkStream networkStream = client.GetStream();

                int i; //reads data as bytes in 256 byte chunks
                while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //appends each byte to string and converts it to ASCII
                    jsonData += Encoding.ASCII.GetString(bytes, 0, i);
                }

                Logger.LogEvent($"Successfully received data of length {jsonData.Length}.");


                try
                {
                    //insert in DB
                    if (saveToDb)
                    {
                        var db = new ServerDatabaseConnection(table, connectionString);
                        db.InsertJsonToDb(jsonData);
                    }
                }
                catch (Exception e)
                {
                    //Log error if something doesn't work during insert (like no valid JSON or other errors)
                    Logger.LogError(e.Message);
                }


                client.Close();
            }
        }

        #endregion
    }
}
