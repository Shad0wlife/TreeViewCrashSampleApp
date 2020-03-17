using DataLibrary.DataAccess.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models.Base
{
    public abstract class UserTrackingModelBase : TrackingModelBase
    {
        [DatabaseProperty("LetzterBearbeiter")]
        public string LetzterBearbeiter { get; set; }
    }
}
