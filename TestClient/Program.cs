using SampleData;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestClient
{
    class Program
    {
        #region Variables
        public enum MenuActions
        {
            random, select, exit, help, clear
        }

        //Action to output in color
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
            ColoredPrint("Welcome to the Test Client!", ConsoleColor.Yellow);
            PrintHelp();

            MenuActions currentAction;

            do
            {
                //get action from user
                currentAction = GetMenuAction();

                switch (currentAction)
                {
                    case MenuActions.random: GenerateSampleDataChoice(); break;
                    case MenuActions.select: SelectTextdataChoice(); break;
                    case MenuActions.help: PrintHelp(); break;
                    case MenuActions.clear: Console.Clear(); break;
                    case MenuActions.exit: break;
                    default: break;
                }

            } while (currentAction != MenuActions.exit); //do until user quits
        }

        #endregion

        #region Methods

        //Help
        private static void PrintHelp()
        {
            Console.WriteLine("\nRandom - Generate random sample data.");
            Console.WriteLine("Select - Select textfile with your own sample data in JSON format.");
            Console.WriteLine("Help - Print all actions");
            Console.WriteLine("Clear - Clear console");
            Console.WriteLine("Exit - End application");
        }

        //Display Menu
        private static MenuActions GetMenuAction()
        {
            MenuActions action = MenuActions.help;

            //ask user to type in action
            ColoredPrint("\nPlease type an action and press enter to continue: ", ConsoleColor.Yellow);
            string choice = Console.ReadLine();

            //if invalid user choice print help
            if (Enum.TryParse(choice.ToLower(), out action) == false)
            {
                ColoredPrint("Invalid choice. Printing help...\n", ConsoleColor.Red);
                action = MenuActions.help;
            }

            return action;
        }

        //Select own json
        private static void SelectTextdataChoice()
        {
            ColoredPrint("\nPlease paste your filepath and hit enter", ConsoleColor.Yellow);
            string path = Console.ReadLine();

            try
            {
                var fileAsArray = File.ReadAllLines(path);
                var file = File.ReadAllText(path);

                //print a preview from file (10 lines)
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

                if (yesno.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    //send file
                    SendToServer(file);
                }
                else
                {
                    //abort
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

        //Random json
        private static void GenerateSampleDataChoice()
        {
            ColoredPrint("\nGenerating random hardware sample data...\n", ConsoleColor.Green);

            //Generate random Hardwaredata
            HardwareData data = new HardwareData();

            //Print json
            Console.WriteLine("Your JSON Data:");
            string json = data.GenerateJson();
            Console.WriteLine(json);

            //Send to server
            SendToServer(json);
        }

        //Send to server
        private static void SendToServer(string jsonString)
        {
            ColoredPrint("\nSending JSON to Server...", ConsoleColor.Green);

            try
            {
                //Get target address info
                var (server, port) = GetServerAndPort();

                //Create TcpClient.
                TcpClient client = new TcpClient(server, port);

                //Get client stream to write data to server
                NetworkStream stream = client.GetStream();

                //Convert ASCII to byte array
                byte[] data = Encoding.ASCII.GetBytes(jsonString);

                //Send json as byte array to connected server (tcplistener) 
                stream.Write(data, 0, data.Length);

                ColoredPrint($"\nSuccessfully sent JSON to {server}:{port}.", ConsoleColor.Green);

                //Close stream and client
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                ColoredPrint($"ArgumentNullException: {e}", ConsoleColor.Red);
            }
            catch (SocketException e)
            {
                ColoredPrint($"SocketException: {e}", ConsoleColor.Red);
            }

        }

        //Ask user for ip and port
        private static (string server, int port) GetServerAndPort()
        {
            string server = string.Empty;
            int port = 0;

            bool validUserInput = false;
            while (!validUserInput)
            {
                ColoredPrint("\nEnter server ip: ", ConsoleColor.Yellow);
                server = Console.ReadLine();
                validUserInput = IPAddress.TryParse(server, out IPAddress _);

                //accept if user types localhost
                if (server == "localhost")
                {
                    server = "127.0.0.1";
                    break;
                }

                if (!validUserInput)
                {
                    ColoredPrint("\nError. Not a valid ip", ConsoleColor.Red);
                }
            }

            validUserInput = false;
            while (!validUserInput)
            {
                ColoredPrint("\nEnter port: ", ConsoleColor.Yellow);

                validUserInput = int.TryParse(Console.ReadLine(), out port);

                if (!validUserInput)
                {
                    ColoredPrint("\nError. Not a valid number", ConsoleColor.Red);
                }
            }

            return (server, port);
        }

        #endregion
    }
}
