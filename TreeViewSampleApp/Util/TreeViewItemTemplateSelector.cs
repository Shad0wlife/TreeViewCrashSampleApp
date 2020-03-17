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
                Debug.Write("Selecting template: ");
                if (node.Content is CheckList)
                {
                    Debug.WriteLine("ListTemplate");
                    return ListTemplate;
                }
                else if (node.Content is CheckPoint)
                {
                    Debug.WriteLine("PointTemplate");
                    return PointTemplate;
                }
            }
            return DefaultTemplate;
        }
    }
}
