using DatabaseAccessLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary
{
    public class ServerDatabaseConnection
    {
        #region Fields
        Dictionary<string, string> existingColumns = new Dictionary<string, string>();
        Dictionary<string, string> nonExistingColumns = new Dictionary<string, string>();
        IEnumerable<KeyValuePair<string, JValue>> fields;
        CRUDLogic db;
        #endregion

        #region Properties
        public string Table { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        #endregion

        public ServerDatabaseConnection(string table, string ip, int port, string cnnString)
        {
            Table = table;
            Ip = ip;
            Port = port;
            db = new CRUDLogic(cnnString);
        }

        public void InsertJsonToDb(string json)
        {
            GetJsonFields(json);
            GetColumnsFromJsonFields();

            string sqlInsertString = SqlStatementHelpers.CreateSqlInsertString(existingColumns, nonExistingColumns, Table);

            if (nonExistingColumns.Count > 0)
            {
                string sqlAlterTableString = SqlStatementHelpers.CreateSqlAlterTableString(nonExistingColumns, Table);
                db.AlterTable(sqlAlterTableString);
            }

            var insertObject = SqlStatementHelpers.CreateNewInsertObject(existingColumns, nonExistingColumns);
            db.Insert(insertObject, sqlInsertString);
        }

        private void GetJsonFields(string json)
        {
            //Deserialize to dynamic object
            dynamic data = JObject.Parse(json);
            JToken jsonParsed = JToken.Parse(json);
            JsonFieldsCollector fieldsCollector = new JsonFieldsCollector(jsonParsed);
            fields = fieldsCollector.GetAllFields();
        }
        private void GetColumnsFromJsonFields()
        {
            foreach (var field in fields)
            {
                //get column name and value
                string columnName = field.Key;
                string value = field.Value.ToString();

                //check if column exists
                bool exists = db.ColumnExists(columnName, Table);

                //todo event for server output
                if (exists)
                {
                    //if it exists, insert
                    //Console.WriteLine($"Column \"{columnName}\" already exists, inserting new value \"{ value }\" ...");
                    existingColumns[columnName] = value;
                }
                else
                {
                    //if not, create and insert
                    //Console.WriteLine($"Column \"{columnName}\" doesn't exist, creating new column...");
                    nonExistingColumns[columnName] = value;
                }
            }
        }




    }
}
