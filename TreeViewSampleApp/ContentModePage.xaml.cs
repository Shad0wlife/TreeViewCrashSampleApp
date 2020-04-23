using DataLibrary.DataAccess;
using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using TreeViewSampleApp.Util;
using TreeViewSampleApp.Util.Wrappers;
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
    public sealed partial class ContentModePage : Page, INotifyPropertyChanged
    {
        public ContentModePage()
        {
            this.DataContext = this;
            this.InitializeComponent();

            this.InitializeView();
        }

        private void InitializeView()
        {

        }


        #region Properties, Vars, Constants

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<CheckList> ComboBoxOptions { get; } = DataStorage.Singleton.getListsbyFilter(null);

        private const int ADD_DELAY = 1;
        private const int CLEAR_DELAY = 20;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private CancellationTokenSource rootCts;
        private CancellationToken rootToken;

        private MUXC.TreeView Tree;

        private CheckList _comboBoxSelected = null;
        public CheckList ComboBoxSelected
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
            if(Combo.SelectedItem == null)
            {
                return;
            }
            if(Tree != null)
            {
                Tree.Expanding -= Tree_Expanding;
                Tree.Collapsed -= Tree_Collapsed;

                Tree.RootNodes.Clear();
            }

            DataStorage.Singleton.resetContents();

            Tree = new MUXC.TreeView();
            object resource;
            this.Resources.TryGetValue("Selector", out resource);
            if (resource is ContentModeItemTemplateSelector selector)
            {
                Tree.ItemTemplateSelector = selector;
            }
            else
            {
                Debug.WriteLine("Selector is no ContentModeItemTemplateSelector!");
            }
            Tree.Expanding += Tree_Expanding;
            Tree.Collapsed += Tree_Collapsed;
            Tree.SelectionMode = MUXC.TreeViewSelectionMode.Single;
            Tree.IsDoubleTapEnabled = false;
            Tree.AllowDrop = true;
            Tree.CanDragItems = false;
            Tree.CanReorderItems = true;
            Tree.ItemsSource = new ObservableCollection<WrapperBase>();

            WrapViewer.Content = Tree;

            this.rootCts = new CancellationTokenSource();
            this.rootToken = rootCts.Token;

            await FillNodeListWithData((ObservableCollection<WrapperBase>)Tree.ItemsSource, this.ComboBoxSelected, rootCts.Token);
        }

        #region Filling

        private async Task<bool> FillNodeListWithData(IList<WrapperBase> targetList, CheckList parent, CancellationToken token)
        {
            targetList.Clear();
            await Task.Delay(CLEAR_DELAY);

            foreach(CheckList c1 in DataStorage.Singleton.getListsbyFilter(parent.List_ID))
            {
                if (token.IsCancellationRequested || rootToken.IsCancellationRequested)
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        targetList.Clear();
                        await Task.Delay(CLEAR_DELAY);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                    return false;
                }
                await semaphore.WaitAsync();
                try
                {
                    targetList.Add(new ListWrapper(c1));
                    await Task.Delay(ADD_DELAY);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            foreach (CheckPoint c2 in DataStorage.Singleton.getPointsbyFilter(parent.List_ID))
            {
                if (token.IsCancellationRequested || rootToken.IsCancellationRequested)
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        targetList.Clear();
                        await Task.Delay(CLEAR_DELAY);
                        return false;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
                await semaphore.WaitAsync();
                try
                {
                    targetList.Add(new PointWrapper(c2));
                    await Task.Delay(ADD_DELAY);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            return true;
        }

        private async Task FillItem(ListWrapper item)
        {
            if (item.HasUnrealizedChildren)
            {
                CancellationToken token = item.TokenSource.Token;

                item.IsWorking = true;
                bool finished = await FillNodeListWithData(item.Children, item.CheckList, token);
                item.IsWorking = false;

                if (finished)
                {
                    item.HasUnrealizedChildren = false;
                }
                else
                {
                    item.HasUnrealizedChildren = true;
                }
            }
        }

        #endregion

        #region Events

        private async void Tree_Expanding(MUXC.TreeView sender, MUXC.TreeViewExpandingEventArgs args)
        {
            if (args.Item is ListWrapper listWrapper && listWrapper.HasUnrealizedChildren)
            {
                
                listWrapper.TokenSource = new CancellationTokenSource();
                listWrapper.Expanded = true;
                await FillItem(listWrapper);
            }
        }

        private void Tree_Collapsed(MUXC.TreeView sender, MUXC.TreeViewCollapsedEventArgs args)
        {
            if (args.Item is ListWrapper listWrapper)
            {
                listWrapper.TokenSource.Cancel();
                listWrapper.Expanded = false;
                listWrapper.Children.Clear();
                listWrapper.HasUnrealizedChildren = true;
            }
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

        private void NodeModeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        #endregion

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

            if (Tree != null)
            {
                Tree.Expanding -= Tree_Expanding;
                Tree.Collapsed -= Tree_Collapsed;

                Tree.RootNodes.Clear();
            }

            this.DataContext = null;
        }

        private async void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rootCts?.Cancel();
            await InitializeTreeView();
        }
    }
}
