using DataLibrary.DataAccess.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace DataLibrary.DataAccess
{
    public class FilterData
    {
        public FilterData(string column, object filter, FilterMethod method)
        {
            Column = column;
            Filter = filter;
            Method = method;
        }
        public string Column { get; set; }
        private object _filter;
        public object Filter
        {
            get
            {
                if (Method == FilterMethod.ISNULL)
                {
                    return "";
                }
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }
        public FilterMethod Method { get; set; }
    }

    public enum FilterMethod
    {
        [Description("=")]
        EQUALS,
        [Description("LIKE")]
        LIKE,
        [Description("GLOB")]
        GLOB,
        [Description("IS NULL")]
        ISNULL
    }

    static class FilterExtension
    {
        public static string GetOperator(this FilterMethod filter)
        {
            Type type = filter.GetType();
            string name = Enum.GetName(type, filter);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        return attribute.Description;
                    }
                }
            }
            return null;
        }
    }

    public class Utility
    {
        public static string GetColumnnameForProperty(PropertyInfo prop)
        {
            DatabasePropertyAttribute att = Attribute.GetCustomAttribute(prop, typeof(DatabasePropertyAttribute)) as DatabasePropertyAttribute;
            return att?.ColumnName;
        }
    }

    public enum OperationResult
    {
        OK, NOT_UNIQUE, ERROR, NONE
    }
}
