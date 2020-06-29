using DatabaseAccessLibrary;
using LogLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
        //public string Ip { get; set; }
        //public int Port { get; set; }
        #endregion

        //inititalizes
        public ServerDatabaseConnection(string table, string cnnString)
        {
            Table = table;
            db = new CRUDLogic(cnnString);
        }

        //method to insert json string in db
        public void InsertJsonToDb(string json)
        {
            //converts json fields to Key value pairs
            GetJsonFields(json);

            //initializes dictionaries of existing + non existing columns
            GetColumnsFromJsonFields();

            //creates insert string
            string sqlInsertString = SqlStatementHelpers.CreateSqlInsertString(existingColumns, nonExistingColumns, Table);

            //creates alter table string for non existing columns
            if (nonExistingColumns.Count > 0)
            {
                Logger.LogMessage("Data contains new Properties. Creating new columns...");

                string sqlAlterTableString = SqlStatementHelpers.CreateSqlAlterTableString(nonExistingColumns, Table);
                db.AlterTable(sqlAlterTableString);
            }

            //creates dynamic object to insert
            var insertObject = SqlStatementHelpers.CreateNewInsertObject(existingColumns, nonExistingColumns);

            Logger.LogMessage("Inserting new data...");

            //insert
            db.Insert(insertObject, sqlInsertString);
        }

        private void GetJsonFields(string json)
        {
            //validate json + parse
            try
            {
                JToken jsonParsed = JToken.Parse(json);
                JsonFieldsCollector fieldsCollector = new JsonFieldsCollector(jsonParsed);
                fields = fieldsCollector.GetAllFields();
            }
            catch (Exception)
            {
                //Throws exception (parsing) + forwards it to the Server 
                //--> doesn't continue + error gets logged
                throw; 
            }

        }
        private void GetColumnsFromJsonFields()
        {
            foreach (var field in fields)
            {
                //get column name and value
                string columnName = field.Key;
                string value = field.Value.ToString();

                //check if column exists
                if (db.ColumnExists(columnName, Table))
                {
                    //if it exists, insert
                    existingColumns[columnName] = value;

                    string message = $"Column \"{columnName}\" already exists, inserting new value \"{ value }\"...";
                    Logger.LogInsert(message);

                }
                else
                {
                    //if not, create and insert
                    nonExistingColumns[columnName] = value;

                    string message = $"Column \"{columnName}\" doesn't exist, creating new column...";
                    Logger.LogCreate(message);
                }
            }
        }




    }
}
