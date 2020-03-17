using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MUXC = Microsoft.UI.Xaml.Controls;

namespace TreeViewSampleApp.Util
{
    class CancellableTreeViewNode : MUXC.TreeViewNode
    {
        public CancellableTreeViewNode() : base()
        {

        }

        public CancellationTokenSource TokenSource { get; set; }
    }
}
