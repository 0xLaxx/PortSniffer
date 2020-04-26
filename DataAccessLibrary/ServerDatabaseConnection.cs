using DatabaseAccessLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataAccessLibrary
{
    public class ServerDatabaseConnection
    {
        #region Fields
        Dictionary<string, string> existingColumns = new Dictionary<string, string>();
        Dictionary<string, string> nonExistingColumns = new Dictionary<string, string>();
        IEnumerable<KeyValuePair<string, JValue>> fields;
        CRUDLogic db;
        public event EventHandler<DbEventArgs> InsertEvent;
        public event EventHandler<DbEventArgs> AlterTableEvent;
        public event EventHandler<DbEventArgs> ErrorEvent;
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
            try
            {
                JToken jsonParsed = JToken.Parse(json);
                JsonFieldsCollector fieldsCollector = new JsonFieldsCollector(jsonParsed);
                fields = fieldsCollector.GetAllFields();
            }
            catch (Exception e)
            {
                //todo - log parsing error

                //Give back error to console on server
                ErrorEvent?.Invoke(this, new DbEventArgs(e.Message));
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
                bool exists = db.ColumnExists(columnName, Table);

                //todo event for server output
                if (exists)
                {
                    //if it exists, insert
                    existingColumns[columnName] = value;

                    string message = $"Column \"{columnName}\" already exists, inserting new value \"{ value }\"...";
                    InsertEvent?.Invoke(this, new DbEventArgs(message));

                }
                else
                {
                    //if not, create and insert
                    nonExistingColumns[columnName] = value;

                    string message = $"Column \"{columnName}\" doesn't exist, creating new column...";
                    AlterTableEvent?.Invoke(this, new DbEventArgs(message));
                }
            }
        }




    }
}
