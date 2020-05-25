using Dapper;
using LogLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DatabaseAccessLibrary
{
    public class CRUDLogic
    {
        public string ConnectionString { get; set; }
        public CRUDLogic(string cnnString)
        {
            ConnectionString = cnnString;
        }

        public List<T> SelectAllFromTable<T>(string table)
        {
            List<T> output = new List<T>();
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    output = connection.Query<T>($"SELECT * FROM {table}").ToList();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }

            return output;
        }

        public void Insert<T>(T p, string sql)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Execute(sql, p, commandType: CommandType.Text);

                    Logger.LogMessage("Data successfully inserted.");
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }
        }

        public void AlterTable(string sqlAlterTableString)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                connection.Execute(sqlAlterTableString, commandType: CommandType.Text);

                Logger.LogMessage("Table successfully altered.");
                Logger.LogMessage(sqlAlterTableString);
            }
        }

        public bool ColumnExists(string columnName, string table)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                List<string> columns = new List<string>();
                try
                {
                    columns = connection.Query<string>($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' ORDER BY ORDINAL_POSITION").ToList();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }


                foreach (var col in columns)
                {
                    if (col.Equals(columnName,StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
    }
}
