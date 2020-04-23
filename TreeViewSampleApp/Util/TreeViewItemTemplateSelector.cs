using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUXC = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using DataLibrary.Models;
using System.Diagnostics;
using TreeViewSampleApp.Util.Nodes;

namespace TreeViewSampleApp.Util
{
    class TreeViewItemTemplateSelector : DataTemplateSelector
    {

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate PointTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is MUXC.TreeViewNode node)
            {
                if (item is CancellableTreeViewNode && node.Content is CheckList)
                {
                    return ListTemplate;
                }
                else if(node.Content is CheckPoint)
                {
                    return PointTemplate;
                }
            }
            return DefaultTemplate;
        }
    }
}
