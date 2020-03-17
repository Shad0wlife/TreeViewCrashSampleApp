using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Attributes;
using DataLibrary.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DataLibrary.Models
{
    [DatabaseTable("CheckList")]
    public class CheckList : UserTrackingModelBase
    {
        [DatabasePropertyPrimarykey]
        [DatabaseProperty("List_ID")]
        public long List_ID { get; set; }

        [DatabasePropertyNonNull]
        [DatabaseProperty("ListName")]
        public string ListName { get; set; }

        [DatabaseProperty("ListParent")]
        public long? ParentList { get; set; }

        public override string ToString()
        {
            return ListName;
        }
    }
}
