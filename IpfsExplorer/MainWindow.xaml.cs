using IpfsExplorer.Core;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IpfsExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<PinnedItem> PinnedItems = new ObservableCollection<PinnedItem>();
        IpfsProxy Proxy;
        public MainWindow()
        {
            InitializeComponent();
            lvPinned.ItemsSource = PinnedItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvPinned.ItemsSource);
            view.SortDescriptions.Add(new SortDescription(nameof(PinnedItem.FileName), ListSortDirection.Ascending));
            Proxy = new IpfsProxy("http://10.0.0.129:5001");
		
        }

        private async void BtnAddFile_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == false) {
                return;
            }
            try {
                var item = await Proxy.AddFileAsync(dialog.FileName);
                PinnedItems.Add(item);
            } catch (Exception ex) {
                MessageBox.Show($"Error adding file: {ex.Message}");
            }
        }

        private async void BtnAddFolder_Click(object sender, RoutedEventArgs e) {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                var item = await Proxy.AddDirectoryAsync(dialog.FileName);
                PinnedItems.Add(item);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var row = sender as ListViewItem;
            if (row != null && row.IsSelected) {
                var item = row.Content as PinnedItem;
                MessageBox.Show(item.Hash);
            }
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnPin_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnPref_Click(object sender, RoutedEventArgs e) {

        }
    }
}
