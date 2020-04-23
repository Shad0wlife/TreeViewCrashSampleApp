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
using TreeViewSampleApp.Util.Wrappers;

namespace TreeViewSampleApp.Util
{
    class ContentModeItemTemplateSelector : DataTemplateSelector
    {

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate PointTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ListWrapper)
            {
                Debug.WriteLine("ListTemplate");
                return ListTemplate;
            }
            else if (item is PointWrapper)
            {
                Debug.WriteLine("PointTemplate");
                return PointTemplate;
            }
            return DefaultTemplate;
        }
    }
}
