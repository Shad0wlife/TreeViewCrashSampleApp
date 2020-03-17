using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    class DatabaseTableAttribute : Attribute
    {
        public string TableName { get; private set; }
        public DatabaseTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
