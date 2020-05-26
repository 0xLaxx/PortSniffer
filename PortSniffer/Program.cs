using LogLibrary;
using System;
using Topshelf;

namespace PortSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitcode = HostFactory.Run(h =>
            {
                h.Service<Server>(s =>
                {
                    s.ConstructUsing(server => new Server());
                    s.WhenStarted(server => server.Start());
                    s.WhenStopped(server => server.Stop());
                });

                h.StartAutomatically();
                h.RunAsLocalSystem();

                h.SetServiceName("Portlistener");
                h.SetDisplayName("Portlistener");

                h.SetDescription("Simple service that listens on a given Port and IP. " +
                                 "Writes all received JSON data into a specified MSSQL Table. Table needs to be created first. " +
                                 "To configure, edit the user.serverconfig in your LocalApplicationData. " +
                                 "This service automatically creates a logfile. To visit, view the port_listener.log in your LocalApplicationData. " +
                                 "Make sure to allow sql connection to NT-AUTHORITY\\SYSTEM in your database settings!");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitcode, exitcode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;

            Logger.LogEvent($"Exited with code {exitCodeValue}");
        }

    }
}