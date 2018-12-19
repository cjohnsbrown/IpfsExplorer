using System;
using System.Collections.Generic;
using System.Linq;
using Ipfs;
using Ipfs.Http;
using Ipfs.CoreApi;
using System.Threading.Tasks;
using System.IO;

namespace IpfsExplorer.Core {
    public class IpfsProxy {

        private IpfsClient Client;

        public IpfsProxy(string host) {
            Client = new IpfsClient(host);
        }

        public async Task<string> AddFileAsync(string filePath) {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                IFileSystemNode node = await Client.FileSystem.AddAsync(stream, Path.GetFileName(filePath));
                return node.Id.Hash.ToString();
            }
        }

        public async Task<string> AddDirectoryAsync(string directoryPath, bool recursive = true) {
            var node = await Client.FileSystem.AddDirectoryAsync(directoryPath, recursive);
            return node.Id.Hash.ToString();
        }

        public async Task<bool> UnpinAsync(string ipfsHash) {
            try {
                Cid id = MakeCid(ipfsHash);
                await Client.Pin.RemoveAsync(id);
                return true;
            } catch {
                return false;
            }
        }

        public async Task<bool> PinAsync(string ipfsHash) {
            try {
                Cid id = MakeCid(ipfsHash);
                await Client.Pin.AddAsync(id);
                return true;
            } catch {
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetPinnedAsync() {
            IEnumerable<Cid> pinned = await Client.Pin.ListAsync();
            return pinned.Select(id => id.Hash.ToString());
        }

        public async Task<Stream> GetFileStream(string ipfsHash) {
            return await Client.FileSystem.ReadFileAsync(ipfsHash);
        }

        public async Task<Stream> GetDirectory(string ipfsHash) {
            return await Client.FileSystem.GetAsync(ipfsHash);
        }

        public async Task<string> GetHash(string filePath) {
            AddFileOptions options = new AddFileOptions { OnlyHash = true };
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                IFileSystemNode node = await Client.FileSystem.AddAsync(stream, Path.GetFileName(filePath), options);
                return node.Id.Hash.ToString();
            }
        }

        private Cid MakeCid(string ipfsHash) {
            return new Cid() {
                Hash = new MultiHash(ipfsHash)
            };
        }


    }
}
