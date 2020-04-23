using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TreeViewSampleApp.Util.Wrappers
{
    public class PointWrapper : WrapperBase
    {
        public CheckPoint CheckPoint { get; }

        public PointWrapper(CheckPoint punkt)
        {
            CheckPoint = punkt;
        }

        public override string ItemName()
        {
            return CheckPoint.PointName;
        }
    }
}
