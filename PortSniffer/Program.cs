using System;
using System.IO;
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

                h.SetDescription("Simple service that listens on a Port and IP. " +
                                 "Writes all received JSON data into a specified MSSQL Table. " +
                                 "To configure, edit the user.serverconfig in your LocalApplicationData.");

            });

            int exitCodeValue = (int)Convert.ChangeType(exitcode, exitcode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }

    }
}
