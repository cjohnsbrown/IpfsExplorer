using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace IpfsExplorer.Core {
    public class ExplorerSettings {


        public const string FILE_NAME = "ipfs-explorer.config";
        public static readonly string DATA_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IpfsExplorer");
        public static string FULL_PATH {
            get {
                return Path.Combine(DATA_PATH, FILE_NAME);
            }
        }

        public string WatchDirectory { get; set; }
        public string ApiHost { get; set; } = "http://localhost:5001";
        public string DownloadsFolder { get; set; } = Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads";
        public string Gateway { get; set; } = "https://ipfs.io";

        public void Save() {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FULL_PATH, json);
        }
    }
}
