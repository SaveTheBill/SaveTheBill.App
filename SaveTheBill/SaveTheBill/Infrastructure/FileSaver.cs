using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace SaveTheBill.Infrastructure
{
    public class FileSaver
    {
        private readonly string _file;
        private readonly string _folder;        
        public FileSaver(string filename = "Data.json", string foldername = "LocalData")
        {
            _file = filename;
            _folder = foldername;
        }

        /// <summary>
        ///     Create and Open the file
        /// </summary>
        /// <returns></returns>
        private async Task<IFile> AssureFileExistsAsync()
        {
            var root = FileSystem.Current.LocalStorage;
            var localFolder =
                await root.CreateFolderAsync(_folder, CreationCollisionOption.OpenIfExists);
            return await localFolder.CreateFileAsync(_file, CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        ///     Reads the Content from the specified file
        /// </summary>
        /// <returns>Method has no return value. Async method have to return a Label</returns>
        public async Task<string> ReadContentFromLocalFileAsync()
        {
            try
            {
                var file = await AssureFileExistsAsync();

                return await file.ReadAllTextAsync();

            }
            catch (Exception e)
            {
                Debug.WriteLine("Error #1: FileSaver.cs: Property may is null or empty");
                Debug.WriteLine(e.InnerException);
            }
            return string.Empty;
        }

        /// <summary>
        ///     Write the content from the online data in the local storage
        /// </summary>
        /// <typeparam name="T">Generic type for more efficiency</typeparam>
        /// <param name="content">Content from the online data</param>
        /// <returns>Method has no return value. Async method have to return a Label</returns>
        public async Task SaveContentToLocalFileAsync<T>(IList<T> content)
        {
            var file = await AssureFileExistsAsync();
            var json = JsonConvert.SerializeObject(content);
            await file.WriteAllTextAsync(json);
        }
    }
}