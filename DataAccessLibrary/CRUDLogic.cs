using Dapper;
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
            using (IDbConnection connection 
                = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    output = connection.Query<T>($"SELECT * FROM {table}").ToList();
                }
                catch (Exception)
                {

                }
            }

            return output;
        }

        public void Insert<T>(T p, string sql)
        {
            using (IDbConnection connection 
                = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                //todo - insert Datetime, ip, port 
                try
                {
                    connection.Execute(sql, p, commandType: CommandType.Text);
                }
                catch (Exception)
                {
                }
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
                List<string> columns = new List<string>();
                try
                {
                    columns = connection.Query<string>($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' ORDER BY ORDINAL_POSITION").ToList();
                }
                catch (Exception e)
                {

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
