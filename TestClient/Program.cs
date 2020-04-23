using SampleData;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TestClient
{
    class Program
    {
        public enum MenuActions
        {
            random, select, exit, help, clear
        }

        public static Action<string, ConsoleColor> ColoredPrint
            = new Action<string, ConsoleColor>((message, color) =>
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            });

        static void Main(string[] args)
        {
            ColoredPrint("Welcome to the Test Client!", ConsoleColor.Yellow);
            PrintHelp();

            MenuActions currentAction;

            do
            {
                currentAction = GetMenuAction();

                switch (currentAction)
                {
                    case MenuActions.random: GenerateSampleDataChoice();  break;
                    case MenuActions.select: SelectTextdataChoice();  break;
                    case MenuActions.help: PrintHelp(); break;
                    case MenuActions.clear: Console.Clear(); break;
                    case MenuActions.exit: break;
                    default: break;
                }

            } while (currentAction != MenuActions.exit);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("\nRandom - Generate random sample data.");
            Console.WriteLine("Select - Select textfile with your own sample data in JSON format.");
            Console.WriteLine("Help - Print all actions");
            Console.WriteLine("Clear - Clear console");
            Console.WriteLine("Exit - End application");
        }

        private static MenuActions GetMenuAction()
        {
            MenuActions action = MenuActions.help;

            ColoredPrint("\nPlease select an action and press enter to continue: ", ConsoleColor.Yellow);
            string choice = Console.ReadLine();

            if (Enum.TryParse(choice.ToLower(), out action) == false)
            {
                ColoredPrint("Invalid choice. Printing help...\n", ConsoleColor.Red);
                action = MenuActions.help;
            }

            return action;
        }

        private static void SelectTextdataChoice()
        {
            ColoredPrint("\nPlease paste your filepath and hit enter", ConsoleColor.Yellow);
            string path = Console.ReadLine();

            try
            {
                var fileAsArray = File.ReadAllLines(path);
                var file = File.ReadAllText(path);

                Console.WriteLine("\nPrinting first 10 lines of your file...");

                for (int i = 0; i < fileAsArray.Length; i++)
                {
                    Console.WriteLine(fileAsArray[i]);

                    if (i > 9)
                    {
                        break;
                    }
                }

                ColoredPrint("\nAre you sure you want to send file to server? (y/n): ", ConsoleColor.Yellow);
                string yesno = Console.ReadLine();

                if (yesno.Equals("y",StringComparison.OrdinalIgnoreCase))
                {
                    SendToServer(file);
                }
                else
                {
                    ColoredPrint("If you say so. ", ConsoleColor.Red);
                    PrintHelp();
                }
            }
            catch (Exception e)
            {
                ColoredPrint($"Something went wrong. Message: {e.Message}", ConsoleColor.Red);
                PrintHelp();
            }

        }

        private static void GenerateSampleDataChoice()
        {
            Console.WriteLine("\nGenerating random hardware sample data...\n");

            //Generate random Hardwaredata
            HardwareData data = new HardwareData();

            //Print json
            Console.WriteLine("Your JSON Data:");
            string json = data.GenerateJson();
            Console.WriteLine(json);

            //Send to server
            SendToServer(json);
        }

        private static void SendToServer(string jsonString)
        {
            //TODO Send json to Server (IP/random Port)   
            ColoredPrint("\nSending JSON to Server...", ConsoleColor.Green);

            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                int port = 55779;
                string server = IPAddress.Loopback.ToString();
                //server = "192.168.178...";

                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonString);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", jsonString);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new byte[256];

                // String to store the response ASCII representation.
                string responseData = string.Empty;

                // Read the first batch of the TcpServer response bytes.
                int bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }
    }
}
