using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Attributes;
using DataLibrary.Models.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    [DatabaseTable("CheckPoint")]
    public class CheckPoint : UserTrackingModelBase
    {
        [DatabasePropertyPrimarykey(false)]
        [DatabasePropertyNonNull]
        [DatabaseProperty("PointName")]
        public string PointName
        {
            get
            {
                return _pointName;
            }
            set
            {
                if (value != _pointName)
                {
                    _pointName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }
        private string _pointName;

        [DatabaseProperty("List_ID")]
        public long List_ID
        {
            get
            {
                return _list_id;
            }
            set
            {
                if (value != _list_id)
                {
                    _list_id = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }
        private long _list_id;

        [DatabasePropertyNonNull]
        [DatabaseProperty("PointDescription")]
        public string PointDescription
        {
            get
            {
                return _pointDesc;
            }
            set
            {
                if (value != _pointDesc)
                {
                    _pointDesc = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }
        private string _pointDesc;

        [DatabaseProperty("HelpText")]
        public string HelpText
        {
            get
            {
                return _helpText;
            }
            set
            {
                if (value != _helpText)
                {
                    _helpText = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }
        private string _helpText;

        [DatabaseProperty("Note")]
        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                if (value != _note)
                {
                    _note = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }
        private string _note;

        [DatabaseProperty("SortingNr")]
        public int SortingNr
        {
            get
            {
                return _sortingNr;
            }
            set
            {
                if (value != _sortingNr)
                {
                    _sortingNr = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }
        private int _sortingNr;

        #region Extra-Information

        private CheckPoint orig;

        #endregion

        #region Functions

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(CheckPoint))
            {
                return false;
            }
            CheckPoint other = (CheckPoint)obj;

            bool result = true;
            result &= PointName == other.PointName;
            result &= List_ID == other.List_ID;
            result &= PointDescription == other.PointDescription;
            result &= HelpText == other.HelpText;
            result &= Note == other.Note;
            result &= SortingNr == other.SortingNr;

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return PointName;
        }

        public CheckPoint clone()
        {
            return new CheckPoint()
            {
                PointName = PointName,
                List_ID = List_ID,
                PointDescription = PointDescription,
                HelpText = HelpText,
                Note = Note
            };
        }

        public void backupToOrig()
        {
            orig = clone();
        }

        public void restoreFromOrig()
        {
            if (orig == null)
            {
                return;
            }

            PointName = orig.PointName;
            List_ID = orig.List_ID;
            PointDescription = orig.PointDescription;
            HelpText = orig.HelpText;
            Note = orig.Note;
        }

        public bool IsChanged
        {
            get
            {
                if (orig == null)
                {
                    return false;
                }
                return !Equals(orig);
            }
        }

        public void UpdateIsChanged()
        {
            OnPropertyChanged(nameof(IsChanged));
        }

        #endregion
    }
}
