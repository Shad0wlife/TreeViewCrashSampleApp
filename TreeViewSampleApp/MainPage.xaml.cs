using DataLibrary.DataAccess;
using DataLibrary.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using TreeViewSampleApp.Util;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace TreeViewSampleApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.InitializeView();
        }

        private void InitializeView()
        {
            Combo.SelectionChanged += async (sender, args) => {
                cts?.Cancel();
                await InitializeTreeView();
            };
        }

        private const int DELAY_LIST = 10;
        private const int DELAY_POINT = 15;
        private const int DELAY_CLEAR = 25;


        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<CheckList> ComboBoxOptions = DataStorage.Singleton.getListsbyFilter(null);

        private CancellationTokenSource cts;

        private CheckList _comboBoxSelected = null;
        private CheckList ComboBoxSelected
        {
            get
            {
                return _comboBoxSelected;
            }
            set
            {
                if(value != _comboBoxSelected)
                {
                    _comboBoxSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MUXC.TreeViewNode SelectedNode
        {
            get
            {
                return Tree.SelectedNode;
            }
        }

        public bool SelectedNodeIsList
        {
            get
            {
                if (Tree.SelectedNode == null)
                {
                    return false;
                }
                return Tree.SelectedNode.Content is CheckList;
            }
        }

        private void UpdateSelectionProperties()
        {
            this.NotifyPropertyChanged(nameof(SelectedNode));
            this.NotifyPropertyChanged(nameof(SelectedNodeIsList));
        }

        #endregion

        private async Task InitializeTreeView()
        {
            this.cts = new CancellationTokenSource();
            await FillNodeListWithData(Tree.RootNodes, this.ComboBoxSelected, cts.Token);
        }

        #region Filling

        private CancellableTreeViewNode CreateListNode(CheckList list)
        {
            CancellableTreeViewNode newNode = new CancellableTreeViewNode();
            newNode.Content = list;
            newNode.HasUnrealizedChildren = true;
            newNode.TokenSource = new CancellationTokenSource();
            return newNode;
        }

        private MUXC.TreeViewNode CreatePointNode(CheckPoint point)
        {
            MUXC.TreeViewNode newNode = new MUXC.TreeViewNode();
            newNode.Content = point;
            return newNode;
        }

        private async Task<bool> FillNodeListWithData(IList<MUXC.TreeViewNode> targetList, CheckList parent, CancellationToken token)
        {
            targetList.Clear();
            await Task.Delay(DELAY_CLEAR);

            foreach(CheckList c1 in DataStorage.Singleton.getListsbyFilter(parent.List_ID))
            {
                if (token.IsCancellationRequested)
                {
                    targetList.Clear();
                    await Task.Delay(DELAY_CLEAR);
                    return false;
                }
                targetList.Add(CreateListNode(c1));
                await Task.Delay(DELAY_LIST);
            }

            foreach (CheckPoint c2 in DataStorage.Singleton.getPointsbyFilter(parent.List_ID))
            {
                if (token.IsCancellationRequested)
                {
                    targetList.Clear();
                    await Task.Delay(DELAY_CLEAR);
                    return false;
                }
                targetList.Add(CreatePointNode(c2));
                await Task.Delay(DELAY_POINT);
            }
            return true;
        }

        private async Task FillNode(MUXC.TreeViewNode node)
        {
            if (node.Content is CheckList && node.HasUnrealizedChildren)
            {
                CheckList list = (CheckList)node.Content;
                CancellationToken token;
                if (node is CancellableTreeViewNode)
                {
                    token = ((CancellableTreeViewNode)node).TokenSource.Token;
                }
                else
                {
                    token = new CancellationToken(false);
                }

                bool finished = await FillNodeListWithData(node.Children, list, token);
                if (finished)
                {
                    node.HasUnrealizedChildren = false;
                }
                else
                {
                    node.HasUnrealizedChildren = true;
                }
            }
        }

        #endregion

        #region Events

        private async void Tree_Expanding(MUXC.TreeView sender, MUXC.TreeViewExpandingEventArgs args)
        {
            if (args.Node.HasUnrealizedChildren)
            {
                if (args.Node is CancellableTreeViewNode)
                {
                    ((CancellableTreeViewNode)args.Node).TokenSource = new CancellationTokenSource();
                }
                await FillNode(args.Node);
            }
        }

        private void Tree_Collapsed(MUXC.TreeView sender, MUXC.TreeViewCollapsedEventArgs args)
        {
            if (args.Node is CancellableTreeViewNode)
            {
                ((CancellableTreeViewNode)args.Node).TokenSource.Cancel();
            }
            args.Node.Children.Clear();
            args.Node.HasUnrealizedChildren = true;
        }

        private void TreeViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                FrameworkElement target = (FrameworkElement)sender;
                if (target is MUXC.TreeViewItem tvi)
                {
                    Tree.SelectedNode = Tree.NodeFromContainer(tvi);
                    UpdateSelectionProperties();
                }
                e.Handled = true;
            }
        }

        private void ExpanderPoint_Expanded(object sender, EventArgs e)
        {

            ((CheckPoint)((MUXC.TreeViewNode)((Expander)sender).DataContext).Content).backupToOrig();
        }

        private void ExpanderPoint_Collapsed(object sender, EventArgs e)
        {
            ((CheckPoint)((MUXC.TreeViewNode)((Expander)sender).DataContext).Content).restoreFromOrig();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MUXC.TreeViewNode node = button.DataContext as MUXC.TreeViewNode;
            if (node != null)
            {
                CheckPoint point = (CheckPoint)node.Content;
                point.backupToOrig();
            }
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ListViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPageListView));
        }

        #endregion

    }
}
