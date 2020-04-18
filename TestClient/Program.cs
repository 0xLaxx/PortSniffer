using SampleData;
using System;
using System.IO;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generate random Hardwaredata
            HardwareData data = new HardwareData();

            //Print json
            Console.WriteLine("Your JSON Data:");
            string json = data.GenerateJson();
            Console.WriteLine(json);

            //Save to textfile
            string id = Guid.NewGuid().ToString();
            File.WriteAllText($"JSON{id}.txt", data.GenerateJson());

            //TODO Send json to Server (IP/random Port)   
            Console.WriteLine("\nSending JSON to Server...");

            Console.Read();
        }
    }
}
