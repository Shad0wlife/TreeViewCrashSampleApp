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
            const int numSelections = 3;
            const int numElemsLarge = 80;
            const int numElemsSmall = 5;

            int mainNum;
            for (mainNum = 0; mainNum < numSelections; mainNum++)
            {
                checkLists.Add(new CheckList() { List_ID = mainNum, ListName = "List" + mainNum, ParentList = null });
            }

            int cntList = mainNum;
            for (int main = 0; main < mainNum-1; main++) {
                for (int cntLoop = 0; cntLoop < numElemsLarge; cntLoop++)
                {
                    checkLists.Add(new CheckList() { List_ID = cntList, ListName = "List" + cntList, ParentList = main });
                    cntList++;
                }
            }

            int cntPoint = 0;
            for (int main = 0; main < mainNum - 1; main++)
            {
                for (int cntLoop = 0; cntLoop < numElemsLarge; cntLoop++)
                {
                    checkPoints.Add(new CheckPoint() { List_ID = main, PointName = "Point"+cntPoint, PointDescription="PointDesc "+cntPoint });
                    cntPoint++;
                }
            }

            //One Selection with few elements
            for (int cntLoop = 0; cntLoop < numElemsSmall; cntLoop++)
            {
                checkLists.Add(new CheckList() { List_ID = cntList, ListName = "List" + cntList, ParentList = mainNum-1 });
                cntList++;
            }

            for (int cntLoop = 0; cntLoop < numElemsSmall; cntLoop++)
            {
                checkPoints.Add(new CheckPoint() { List_ID = mainNum-1, PointName = "Point" + cntPoint, PointDescription = "PointDesc " + cntPoint });
                cntPoint++;
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
