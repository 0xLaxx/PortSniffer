using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace DataAccess
{
    class Program //TODO --> Class Library statt Console
    {
        static void Main(string[] args)
        {
            string json = File.ReadAllText(@"C:\Users\Christoph\source\repos\HSMW\PortSniffer\Client\bin\Debug\netcoreapp3.1\json.txt");
            Console.WriteLine(json);

            //Deserialize to dynamic object
            dynamic data = JObject.Parse(json);

            //show some (nested) data --> each property will be a column?
            Console.WriteLine(data.Harddrives[0].Name);
            Console.WriteLine(data.Username+"\n");

            var jsonParsed = JToken.Parse(json);
            var fieldsCollector = new JsonFieldsCollector(jsonParsed);
            var fields = fieldsCollector.GetAllFields();

            foreach (var field in fields)
            {
                Console.WriteLine($"{field.Key}: {field.Value}");
            }



            // MSSQL Express Connection Test
            DatabaseAccess db = new DatabaseAccess();
            var test = db.GetData();

            //insert test with dummy data
            //Person p = new Person { Age = "23", Name = "Hogus" };
            //db.Insert(p);

            foreach (var field in fields)
            {
                //get column name and value
                string columnName = field.Key;
                string value = field.Value.ToString();

                //check if column exists
                bool exists = db.DoesColumnExist(columnName);

                if (exists)
                {
                    //if it exists, insert
                    Console.WriteLine($"Column \"{field.Key}\" already exists, inserting.");
                }
                else
                {
                    //if not, create and insert
                    Console.WriteLine($"Column \"{field.Key}\" doesn't exist, creating new column...");
                }
            }

            Console.ReadLine();
        }

    }
}
