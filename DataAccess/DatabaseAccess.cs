using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DatabaseAccess
    {
        string connectionString = @"Server=localhost\SQLEXPRESS;Database=TestDb;Trusted_Connection=True;";

        public List<Person> GetData()
        {
            List<Person> output = new List<Person>();
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                output = connection.Query<Person>("Select * From Person")
                                      .ToList();

            }

            return output;
        }

        public void Insert(Person p)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Execute($"Insert into Person (Name, Age) values (@Name,@Age)", 
                    p,
                    commandType: CommandType.Text);
            }
        }

        public bool DoesColumnExist(string columnName)
        {
            bool exists = false;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                var columns = connection.Query<string>("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Person' ORDER BY ORDINAL_POSITION").ToList();

                foreach (var col in columns)
                {
                    if (col.Equals(columnName,StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        return exists;
                    }
                }

            }

            return false;

        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Age { get; set; }

    }
}
