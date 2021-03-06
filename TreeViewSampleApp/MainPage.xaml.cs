﻿using DataLibrary.DataAccess;
using DataLibrary.Models;
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
using TreeViewSampleApp.Util.Nodes;
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
            if (resource is TreeViewItemTemplateSelector selector)
            {
                Tree.ItemTemplateSelector = selector;
            }
            else
            {
                Debug.WriteLine("Selector is no TreeViewItemTemplateSelector!");
            }
            Tree.Expanding += Tree_Expanding;
            Tree.Collapsed += Tree_Collapsed;
            Tree.SelectionMode = MUXC.TreeViewSelectionMode.Single;
            Tree.IsDoubleTapEnabled = false;
            Tree.AllowDrop = true;
            Tree.CanDragItems = false;
            Tree.CanReorderItems = true;

            WrapViewer.Content = Tree;

            this.rootCts = new CancellationTokenSource();
            this.rootToken = rootCts.Token;

            await FillNodeListWithData(Tree.RootNodes, this.ComboBoxSelected, rootCts.Token);
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
                    targetList.Add(CreateListNode(c1));
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
                    targetList.Add(CreatePointNode(c2));
                    await Task.Delay(ADD_DELAY);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            return true;
        }

        private async Task FillNode(MUXC.TreeViewNode node)
        {
            if (node is CancellableTreeViewNode ctvn && node.Content is CheckList list && node.HasUnrealizedChildren)
            {
                CancellationToken token = ctvn.TokenSource.Token;

                ctvn.IsWorking = true;
                bool finished = await FillNodeListWithData(node.Children, list, token);
                ctvn.IsWorking = false;

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
                if (args.Node is CancellableTreeViewNode ctvn)
                {
                    ctvn.TokenSource = new CancellationTokenSource();
                    ctvn.IsExpanded = true;
                }
                await FillNode(args.Node);
            }
        }

        private void Tree_Collapsed(MUXC.TreeView sender, MUXC.TreeViewCollapsedEventArgs args)
        {
            if (args.Node is CancellableTreeViewNode ctvn)
            {
                ctvn.TokenSource.Cancel();
                ctvn.IsExpanded = false;
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

        private void ContentModeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ContentModePage));
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
