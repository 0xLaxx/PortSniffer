using Newtonsoft.Json;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleData data = new SampleData();
            string json = data.ToJson();

            Console.WriteLine(json);
            
            //TODO Send json to Server (IP/random Port)   
            
            Console.Read();
        }
    }
}
