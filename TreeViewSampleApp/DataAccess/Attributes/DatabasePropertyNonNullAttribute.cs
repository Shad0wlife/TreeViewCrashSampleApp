using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class DatabasePropertyNonNullAttribute : Attribute
    {
        public DatabasePropertyNonNullAttribute()
        {
        }
    }
}
