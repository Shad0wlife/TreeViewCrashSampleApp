using DataLibrary.DataAccess.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models.Base
{
    public abstract class TrackingModelBase : ModelBase
    {
        [DatabaseProperty("Aenderungsdatum")]
        public DateTime Aenderungsdatum { get; set; }

        [DatabaseProperty("OfflineGespeichert")]
        public bool OfflineGespeichert { get; set; }
    }
}
