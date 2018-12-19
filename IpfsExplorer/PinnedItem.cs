using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace IpfsExplorer
{
    class PinnedItem { 
        public string Hash { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime PinDate { get; set; }
        public bool IsDirectory { get; set; }

        public PinnedItem() { }

        public PinnedItem(string hash, FileInfo info) {
            Hash = hash;
            FileName = info.Name;
            Size = info.Length;
            PinDate = DateTime.Now;
            IsDirectory = false;
        }

        public PinnedItem(string hash, DirectoryInfo info) {
            Hash = hash;
            FileName = info.Name;
            PinDate = DateTime.Now;
            IsDirectory = true;

            Size = GetDirectorySize(info);
        }

        private long GetDirectorySize(DirectoryInfo info) {
            long size = info.EnumerateFiles().Sum(f => f.Length);

            foreach (var d in info.EnumerateDirectories()) {
                size += GetDirectorySize(d);
            }
            return size;
        }
        
    }
}
