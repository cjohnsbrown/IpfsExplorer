using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Ipfs;
using System.ComponentModel;

namespace IpfsExplorer
{
    public class PinnedItem : INotifyPropertyChanged {

        private readonly string[] Units = new[] { "kB", "MB", "GB", "TB" };

        private string name;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Hash { get; set; }

        public string FileName {
            get {
                return name;
            }
            set {
                name = value;
                NotifyPropertyChanged(nameof(FileName));
            }
        }

        public string Size { get; set; }
        public DateTime PinDate { get; set; }
        public bool IsDirectory { get; set; }

        public List<PinnedItem> DirectoryContents { get; private set; }

        public PinnedItem() { }


        public PinnedItem(IFileSystemNode node) {
            Hash = node.Id.Hash.ToBase58();
            FileName = node.ToLink().Name;
            PinDate = DateTime.Now;
            IsDirectory = true;

            //Convert size to human readable string
            var i = -1;
            double displaySize = node.Size;
            do {
                displaySize /= 1024;
                i++;
            } while (displaySize > 1024 && i < Units.Length);

            Size = $"{Math.Max(displaySize, 0.1).ToString("0.#")} {Units[i]}";

            DirectoryContents = new List<PinnedItem>(node.Links.Count());
            foreach (var link in node.Links) {
                DirectoryContents.Add(new PinnedItem(link));
            }

        }

        public PinnedItem(IFileSystemLink link) {
            Hash = link.Id.Hash.ToBase58();
            FileName = link.Name;
            PinDate = DateTime.Now;
            IsDirectory = false;

            //Convert size to human readable string
            var i = -1;
            double displaySize = link.Size;
            do {
                displaySize /= 1024;
                i++;
            } while (displaySize > 1024 && i < Units.Length);

            Size = $"{Math.Max(displaySize, 0.1).ToString("0.#")} {Units[i]}";
        }

        private void NotifyPropertyChanged(string propName) {
            if (PropertyChanged !=null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
