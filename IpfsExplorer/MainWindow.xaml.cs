using IpfsExplorer.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<PinnedItem> items = new ObservableCollection<PinnedItem>();
        public MainWindow()
        {
            InitializeComponent();

            lvPinned.ItemsSource = items;
            Run();
		
        }

        public async Task Run() {

            var path = @"C:\Users\cameron\mine\config.txt";
            var proxy = new IpfsProxy("http://10.0.0.129:5001");
            string hash = await proxy.AddFileAsync(path);
            items.Add(new PinnedItem(hash, new System.IO.FileInfo(path)));

        }
    }
}
