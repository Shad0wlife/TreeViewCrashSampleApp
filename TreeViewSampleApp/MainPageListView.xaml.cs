using DataLibrary.DataAccess;
using DataLibrary.Models;
using DataLibrary.Models.Base;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
    public sealed partial class MainPageListView : Page, INotifyPropertyChanged
    {
        public MainPageListView()
        {
            this.InitializeComponent();

            this.InitializeView();
        }

        private void InitializeView()
        {
            Combo.SelectionChanged += async (sender, args) => {
                cts?.Cancel();
                await InitializeListView();
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
        private ListView MainList;

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

        public ListViewItem SelectedItem
        {
            get
            {
                return MainList.SelectedItem as ListViewItem;
            }
        }

        public bool SelectedItemIsChecklist
        {
            get
            {
                if (MainList.SelectedItem as ListViewItem == null)
                {
                    return false;
                }
                return ((ListViewItem)MainList.SelectedItem).Content is CheckList;
            }
        }

        private void UpdateSelectionProperties()
        {
            this.NotifyPropertyChanged(nameof(SelectedItem));
            this.NotifyPropertyChanged(nameof(SelectedItemIsChecklist));
        }

        #endregion

        private async Task InitializeListView()
        {
            MainList = new ListView();
            MainList.ItemsSource = new ObservableCollection<ModelBase>();
            object resource;
            this.Resources.TryGetValue("Selector", out resource);
            if(resource is ListViewItemTemplateSelector selector)
            {
                MainList.ItemTemplateSelector = selector;
            }
            else
            {
                Debug.WriteLine("Selector is no ListViewItemTemplateSelector!");
            }
            WrapViewer.Content = MainList;
            

            this.cts = new CancellationTokenSource();
            await FillListWithData((IList<ModelBase>)MainList.ItemsSource, this.ComboBoxSelected, cts.Token);
        }

        #region Filling

        private async Task<bool> FillListWithData(IList<ModelBase> targetList, CheckList parent, CancellationToken token)
        {
            targetList.Clear();
            await Task.Delay(DELAY_CLEAR);

            foreach (CheckList c1 in DataStorage.Singleton.getListsbyFilter(parent.List_ID))
            {
                if (token.IsCancellationRequested)
                {
                    targetList.Clear();
                    await Task.Delay(DELAY_CLEAR);
                    return false;
                }
                targetList.Add(c1);
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
                targetList.Add(c2);
                await Task.Delay(DELAY_POINT);
            }
            return true;
        }

        #endregion

        #region Events

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                FrameworkElement target = (FrameworkElement)sender;
                if (target is MUXC.TreeViewItem tvi)
                {
                    ListViewItem item = MainList.SelectedItem as ListViewItem;
                    UpdateSelectionProperties();
                }
                e.Handled = true;
            }
        }

        private void ExpanderPoint_Expanded(object sender, EventArgs e)
        {

            ((CheckPoint)((Expander)sender).DataContext).backupToOrig();
        }

        private void ExpanderPoint_Collapsed(object sender, EventArgs e)
        {
            ((CheckPoint)((Expander)sender).DataContext).restoreFromOrig();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CheckPoint point = button.DataContext as CheckPoint;
            if (point != null)
            {
                point.backupToOrig();
            }
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TreeViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

    }
}
