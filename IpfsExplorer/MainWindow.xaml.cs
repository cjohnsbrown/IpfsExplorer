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

namespace IpfsExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ExplorerModel Model;

        public MainWindow()
        {
            InitializeComponent();

            Model = new ExplorerModel();
            Model.Init();

            // CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvPinned.ItemsSource);
            // view.SortDescriptions.Add(new SortDescription(nameof(PinnedItem.FileName), ListSortDirection.Ascending));

        }

        private async void BtnAddFile_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == false) {
                return;
            }
            try {
                await Model.AddFileAsync(dialog.FileName);
            } catch (Exception ex) {
                MessageBox.Show($"Error adding file: {ex.Message}");
            }
        }

        private async void BtnAddFolder_Click(object sender, RoutedEventArgs e) {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                await Model.AddDirectoryAsync(dialog.FileName);
            }
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e) {

        }

        private async void BtnPin_Click(object sender, RoutedEventArgs e) {
            var dialog = new InputDialog("Enter IPFS hash:");
            try {
                if (dialog.ShowDialog() == true) {
                    await Model.PinAsync(dialog.Answer);
                }
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, "Error pinning item", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnRemove_Click(object sender, RoutedEventArgs e) {
            var item = lvPinned.SelectedItem as PinnedItem;
            await Model.RemoveAndUnpinItemAsync(item);
        }

        private void BtnPref_Click(object sender, RoutedEventArgs e) {
            var window = new PreferencesWindow(Model.Settings);
            window.ShowDialog();
        }


        private async void Context_SaveAs_Click(object sender, RoutedEventArgs e) {
            var item = lvPinned.SelectedItem as PinnedItem;
            if (item.IsDirectory) {
                var folderDialog = new CommonOpenFileDialog();
                folderDialog.IsFolderPicker = true;
                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok) {
                    await Model.SaveDirectoryAsync(folderDialog.FileName, item);
                }
                return;
            }

            //item is a file
            var dialog = new SaveFileDialog();
            dialog.FileName = item.FileName;
            dialog.InitialDirectory = Model.Settings.DownloadsFolder;
            if (dialog.ShowDialog() == true) {
                await Model.SaveFileAsync(dialog.FileName, item);
            }
        }

        private async void Context_Remove_Click(object sender, RoutedEventArgs e) {
            var item = lvPinned.SelectedItem as PinnedItem;
            await Model.RemoveAndUnpinItemAsync(item);
        }

        private void Context_Copy_Click(object sender, RoutedEventArgs e) {
            var item = lvPinned.SelectedItem as PinnedItem;
            TextCopy.Clipboard.SetText(item.Hash);
        }

        private void Context_Open_Click(object sender, RoutedEventArgs e) {
            var item = lvPinned.SelectedItem as PinnedItem;
            Model.OpenInBrowser(item);

        }

        private void Context_Rename_Click(object sender, RoutedEventArgs e) {
            var item = lvPinned.SelectedItem as PinnedItem;
            var dialog = new InputDialog("Enter new file name:", item.FileName);
            dialog.Title = "Rename";
            if (dialog.ShowDialog() == true) {
                Model.RenameItem(item, dialog.Answer);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {

            await Model.SyncPinned();
            lvPinned.ItemsSource = Model.PinnedItems;
        }
    }
}
