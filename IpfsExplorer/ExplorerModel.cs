using IpfsExplorer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IpfsExplorer {
    class ExplorerModel {


        public ExplorerSettings Settings { get; private set; }
        private IpfsProxy Proxy { get; set;}
        public ObservableCollection<PinnedItem> PinnedItems { get; private set; }

        private string PinnedItemsFile {
            get {
                string fileName = "localhost";
                if (Settings.ApiHost.StartsWith("http")) {
                    fileName = Settings.ApiHost.Replace("http://", string.Empty).Replace(":", "_");
                } else if (Settings.ApiHost.StartsWith("https")) {
                    fileName = Settings.ApiHost.Replace("https://", string.Empty).Replace(":", "_");
                }
                return $"{ExplorerSettings.DATA_PATH}\\{fileName}";
            }
        }

        public void InitAsync() {
            LoadSettings();
            Proxy = new IpfsProxy(Settings.ApiHost);

            if (!File.Exists(PinnedItemsFile)) {
                PinnedItems = new ObservableCollection<PinnedItem>();
                return;
            }

            //Read in pinned items that match this host
            string json = File.ReadAllText(PinnedItemsFile);
            var pinnedOnClient = JsonConvert.DeserializeObject<IEnumerable<PinnedItem>>(json);
            PinnedItems = new ObservableCollection<PinnedItem>(pinnedOnClient);
        }

        public void LoadSettings() {
            if (!Directory.Exists(ExplorerSettings.DATA_PATH)) {
                Directory.CreateDirectory(ExplorerSettings.DATA_PATH);
                Settings = new ExplorerSettings();
                Settings.Save();
                return;
            }

            if(!File.Exists(ExplorerSettings.FULL_PATH)) {
                Settings = new ExplorerSettings();
                Settings.Save();
                return;
            }

            string json = File.ReadAllText($"{ExplorerSettings.DATA_PATH}\\{ExplorerSettings.FILE_NAME}");
            Settings = JsonConvert.DeserializeObject<ExplorerSettings>(json);
        }

        public async Task AddFileAsync(string path) {
            PinnedItem item = await Proxy.AddFileAsync(path);
            PinnedItems.Add(item);

            string json = JsonConvert.SerializeObject(PinnedItems.ToList());
            File.WriteAllText(PinnedItemsFile, json);
        }

        public void RemoveItem(PinnedItem item) {
            PinnedItems.Remove(item);
        }

        public async Task RemoveAndUnpinItemAsync(PinnedItem item) {
            if (await Proxy.UnpinAsync(item.Hash)) {
                PinnedItems.Remove(item);
                string json = JsonConvert.SerializeObject(PinnedItems.ToList());
                File.WriteAllText(PinnedItemsFile, json);
            }
        }

        public async Task SaveFile(string path, PinnedItem item) {
            var stream = await Proxy.GetFileStream(item.Hash);
            using (var file = File.Create(path)) {
                stream.CopyTo(file);
            }
        }

        public void OpenInBrowser(PinnedItem item) {
            var url = $"{Settings.Gateway}/ipfs/{item.Hash}";
            var process = new Process();
            var info = new ProcessStartInfo("cmd", $"/c start {url}") {
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Process.Start(info);
        }

        public async Task Open(PinnedItem item) {
            var path = Path.Combine(Path.GetTempPath(), item.FileName);
            await SaveFile(path, item);
            var process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }
}
