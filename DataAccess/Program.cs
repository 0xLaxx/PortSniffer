using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataAccess
{
    class Program //TODO --> Class Library statt Console
    {
        static void Main(string[] args)
        {
            string table = "Person";
            string json = File.ReadAllText(@"C:\Users\Christoph\source\repos\HSMW\PortSniffer\TestClient\bin\Debug\JSON5bbe56fc-8f03-43b5-9975-ecfbb39dccc2.txt");
            Console.WriteLine(json);

            //Deserialize to dynamic object
            dynamic data = JObject.Parse(json);

            var jsonParsed = JToken.Parse(json);
            var fieldsCollector = new JsonFieldsCollector(jsonParsed);
            var fields = fieldsCollector.GetAllFields();

            foreach (var field in fields)
            {
                Console.WriteLine($"{field.Key}: {field.Value}");
            }

            // MSSQL Express Connection Test
            DatabaseAccess db = new DatabaseAccess();

            //two lists with existing and non existing
            var existingColumns = new Dictionary<string, string>();
            var nonExistingColumns = new Dictionary<string, string>();

            //TODO Use Logger, not Console.WriteLine
            foreach (var field in fields)
            {
                //get column name and value
                string columnName = field.Key;
                string value = field.Value.ToString();

                //check if column exists
                bool exists = db.ColumnExists(columnName, table);
                
                if (exists)
                {
                    //if it exists, insert
                    Console.WriteLine($"Column \"{columnName}\" already exists, inserting new value \"{ value }\" ...");
                    existingColumns[columnName] = value;
                }
                else
                {
                    //if not, create and insert
                    Console.WriteLine($"Column \"{columnName}\" doesn't exist, creating new column...");
                    nonExistingColumns[columnName] = value;
                }
            }

            string sqlInsertString = SqlStatementHelpers.CreateSqlInsertString(existingColumns, nonExistingColumns, table);

            if (nonExistingColumns.Count > 0)
            {
                string sqlAlterTableString = SqlStatementHelpers.CreateSqlAlterTableString(nonExistingColumns, table);
                db.AlterTable(sqlAlterTableString);
            }

            var insertObject = SqlStatementHelpers.CreateNewInsertObject(existingColumns, nonExistingColumns);
            db.Insert(insertObject, sqlInsertString);

            Console.ReadLine();
        }

        
    }
}
