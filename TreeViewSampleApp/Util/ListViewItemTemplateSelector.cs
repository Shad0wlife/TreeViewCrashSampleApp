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
using DataLibrary.Models.Base;

namespace TreeViewSampleApp.Util
{
    class ListViewItemTemplateSelector : DataTemplateSelector
    {

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate PointTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ModelBase node)
            {
                Debug.Write("Selecting template: ");
                if (node is CheckList)
                {
                    Debug.WriteLine("ListTemplate");
                    return ListTemplate;
                }
                else if (node is CheckPoint)
                {
                    Debug.WriteLine("PointTemplate");
                    return PointTemplate;
                }
            }
            return DefaultTemplate;
        }
    }
}
