using Newtonsoft.Json;
using System;
using System.IO;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleData data = new SampleData();
            string json = data.ToJson();

            Console.WriteLine(json);
            File.WriteAllText("json.txt", json);
            //TODO Send json to Server (IP/random Port)   
            
            Console.Read();
        }
    }
}
