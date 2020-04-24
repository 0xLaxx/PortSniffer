using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DatabaseAccessLibrary
{
    public class CRUDLogic
    {
        //string connectionString 
        //    = @"Server=localhost\SQLEXPRESS;Database=TestDb;Trusted_Connection=True;";
        public string ConnectionString { get; set; }
        public CRUDLogic(string cnnString)
        {
            ConnectionString = cnnString;
        }

        public List<T> SelectAllFromTable<T>(string table)
        {
            List<T> output = new List<T>();
            using (IDbConnection connection 
                = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                output = connection.Query<T>($"SELECT * FROM {table}").ToList();
            }

            return output;
        }

        public void Insert<T>(T p, string sql)
        {
            using (IDbConnection connection 
                = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                //TODO Datetime insert
                connection.Execute(sql, p, commandType: CommandType.Text);
            }
        }

        public void AlterTable(string sqlAlterTableString)
        {
            using (IDbConnection connection 
                = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                connection.Execute(sqlAlterTableString, commandType: CommandType.Text);
            }
        }

        public bool ColumnExists(string columnName, string table)
        {
            using (IDbConnection connection 
                = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                var columns = connection.Query<string>($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' ORDER BY ORDINAL_POSITION").ToList();

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
