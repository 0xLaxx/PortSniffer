using SampleData;
using System;
using System.IO;
using System.Linq;

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
        }
    }
}
