//#define DELAY

using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace DataLibrary.DataAccess
{
    public class DataStorage
    {
        private static DataStorage _singleton;
        public static DataStorage Singleton
        {
            get
            {
                if(_singleton == null)
                {
                    _singleton = new DataStorage();
                }
                return _singleton;
            }
        }

        private DataStorage()
        {
            checkLists.Add(new CheckList() { List_ID = 0, ListName = "List0", ParentList = null });
            checkLists.Add(new CheckList() { List_ID = 1, ListName = "List1", ParentList = null });
            checkLists.Add(new CheckList() { List_ID = 2, ListName = "List2", ParentList = null });

            int cntList = 3;
            for (int main = 0; main <= 2; main++) {
                for (int cntLoop = 0; cntLoop < 50; cntLoop++)
                {
                    checkLists.Add(new CheckList() { List_ID = cntList, ListName = "List" + cntList, ParentList = main });
                    cntList++;
                }
            }

            int cntPoint = 0;
            for (int main = 0; main <= 2; main++)
            {
                for (int cntLoop = 0; cntLoop < 50; cntLoop++)
                {
                    checkPoints.Add(new CheckPoint() { List_ID = main, PointName = "Point"+cntPoint, PointDescription="PointDesc "+cntPoint });
                    cntPoint++;
                }
            }
        }

        public List<CheckList> checkLists = new List<CheckList>();
        public List<CheckPoint> checkPoints = new List<CheckPoint>();

        public List<CheckList> getListsbyFilter(long? parent)
        {
            return checkLists.FindAll((l) => { return l.ParentList == parent; });
        }

        public List<CheckPoint> getPointsbyFilter(long? parent)
        {
            return checkPoints.FindAll((p) => { return p.List_ID == parent; });
        }
    }
}
