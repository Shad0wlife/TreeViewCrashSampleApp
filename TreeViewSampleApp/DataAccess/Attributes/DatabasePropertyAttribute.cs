using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class DatabasePropertyAttribute : Attribute
    {
        public string ColumnName { get; private set; }
        public DatabasePropertyAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
