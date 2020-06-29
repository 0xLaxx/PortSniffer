using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace DatabaseAccessLibrary
{
    public static class SqlStatementHelpers
    {
        //loop through non existing, get all keys and create alter table statement
        public static string CreateSqlAlterTableString(Dictionary<string, string> nonExistingColumns, string table)
        {
            if (nonExistingColumns.Count == 0)
            {
                throw new Exception("There are no new Columns to add. Can't create valid alter table string.");
            }

            string alterTableString = $"ALTER TABLE {table} ADD ";

            foreach (var col in nonExistingColumns)
            {
                alterTableString += $"{col.Key} VARCHAR(MAX),";
            }

            //remove last ,
            alterTableString = alterTableString.Substring(0, alterTableString.LastIndexOf(','));
            alterTableString += ";";

            return alterTableString;
        }

        //loop through both dictionaries, create insert statement
        public static string CreateSqlInsertString(Dictionary<string, string> existingColumns
            , Dictionary<string, string> nonExistingColumns, string table)
        {
            if (nonExistingColumns.Count == 0 && existingColumns.Count == 0)
            {
                throw new Exception("There are no Values to Insert. Can't create valid insert string.");
            }

            string insertString = $"INSERT INTO {table} (";
            var mergedDictionaries = existingColumns.Union(nonExistingColumns);

            foreach (var col in mergedDictionaries)
            {
                insertString += $"{col.Key},";
            }

            //remove last ,
            insertString = insertString.Substring(0, insertString.LastIndexOf(','));
            insertString += ") VALUES (";

            foreach (var col in mergedDictionaries)
            {
                insertString += $"@{col.Key},";
            }

            //remove last ,
            insertString = insertString.Substring(0, insertString.LastIndexOf(','));
            insertString += ")";

            return insertString;
        }

        public static dynamic CreateNewInsertObject(Dictionary<string, string> existingColumns
            , Dictionary<string, string> nonExistingColumns)
        {
            //merges dictionaries
            var mergedDictionaries = existingColumns.Union(nonExistingColumns);
            
            //converts it to string, object dictionary --> is needed for expandoobject
            var dictionary = mergedDictionaries.ToDictionary(pair => pair.Key, pair => (object)pair.Value);

            var expandoObject = new ExpandoObject();
            var expandoObjectCollection = (ICollection<KeyValuePair<string, object>>)expandoObject;

            //maps each key to expandoobjectcollection
            //like creating a new class dynamically with propertyname=key and propertyvalue=value
            foreach (var item in dictionary)
            {
                expandoObjectCollection.Add(item);
            }

            //convert to dynamic
            dynamic output = expandoObject;
            return output;
        }
    }
}
