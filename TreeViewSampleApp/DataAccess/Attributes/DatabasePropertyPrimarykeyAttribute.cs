using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class DatabasePropertyPrimarykeyAttribute : Attribute
    {
        public uint MultiIndex { get; private set; }
        public bool Autoincrementing { get; private set; }

        /// <summary>
        /// Default constructor - MultiIndex is set to 0 and autoincrement to true
        /// </summary>
        public DatabasePropertyPrimarykeyAttribute()
        {
            MultiIndex = 0;
            Autoincrementing = true;
        }

        /// <summary>
        /// Constructor for Single PKs with explicit autoincrement info
        /// </summary>
        /// <param name="autoincrementing">Whether the property is autoincremented by the db</param>
        public DatabasePropertyPrimarykeyAttribute(bool autoincrementing)
        {
            MultiIndex = 0;
            Autoincrementing = autoincrementing;
        }

        /// <summary>
        /// Constructor for Combined PKs (or with explicit index info).
        /// WARNING: Autoincrement is set to false here - combined keys usually don't autoincrement parts of them.
        /// </summary>
        /// <param name="index">The index in the PK order</param>
        public DatabasePropertyPrimarykeyAttribute(uint index)
        {
            MultiIndex = index;
            Autoincrementing = false;
        }

        /// <summary>
        /// Allows to give the index of this Property in a combined Primary key
        /// </summary>
        /// <param name="index">The index in the PK order</param>
        /// <param name="autoincrementing">Whether the property is autoincremented by the db</param>
        public DatabasePropertyPrimarykeyAttribute(uint index, bool autoincrementing)
        {
            MultiIndex = index;
            Autoincrementing = autoincrementing;
        }
    }
}
