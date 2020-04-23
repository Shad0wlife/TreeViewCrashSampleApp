using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MUXC = Microsoft.UI.Xaml.Controls;

namespace TreeViewSampleApp.Util.Nodes
{
    class CancellableTreeViewNode : MUXC.TreeViewNode, INotifyPropertyChanged
    {
        public CancellableTreeViewNode() : base()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public CancellationTokenSource TokenSource { get; set; }

        private bool _isWorking = false;
        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set
            {
                if (value != _isWorking)
                {
                    _isWorking = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
