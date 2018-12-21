using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Ipfs;

namespace IpfsExplorer
{
    public class PinnedItem {

        private readonly string[] Units = new[] { "kB", "MB", "GB", "TB" };

        public string Hash { get; set; }
        public string FileName { get; set; }
        public string Size { get; set; }
        public DateTime PinDate { get; set; }
        public bool IsDirectory { get; set; }

        public PinnedItem() { }


        public PinnedItem(IFileSystemNode node) {
            Hash = node.Id.Hash.ToBase58();
            FileName = node.ToLink().Name;
            PinDate = DateTime.Now;
            IsDirectory = node.IsDirectory;

            //Convert size to human readable string
            var i = -1;
            double displaySize = node.Size;
            do {
                displaySize /= 1024;
                i++;
            } while (displaySize > 1024 && i < Units.Length);

            Size = $"{Math.Max(displaySize, 0.1).ToString("0.#")} {Units[i]}";

        }
        
    }
}
