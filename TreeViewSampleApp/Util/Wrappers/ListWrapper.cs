using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TreeViewSampleApp.Util.Wrappers
{
    public class ListWrapper : WrapperBase, INotifyPropertyChanged
    {
        public CheckList CheckList { get; }
        public CancellationTokenSource TokenSource { get; set; }
        private bool isWorking;
        public bool IsWorking
        {
            get
            {
                return isWorking;
            }
            set
            {
                if (value != isWorking)
                {
                    isWorking = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<WrapperBase> Children { get; }
        private bool hasUnrealizedChildren;
        public bool HasUnrealizedChildren
        {
            get
            {
                return hasUnrealizedChildren;
            }
            set
            {
                if (value != hasUnrealizedChildren)
                {
                    hasUnrealizedChildren = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool expanded;
        public bool Expanded
        {
            get
            {
                return expanded;
            }
            set
            {
                if (value != expanded)
                {
                    expanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public ListWrapper(CheckList liste)
        {
            CheckList = liste;
            TokenSource = null;
            IsWorking = false;
            Children = new ObservableCollection<WrapperBase>();
            HasUnrealizedChildren = true;
            Expanded = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public override string ItemName()
        {
            return CheckList.ListName;
        }
    }
}
