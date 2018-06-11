using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Plato.Internal.FileSystem.Abstractions
{
    public interface IAppDataFolder
    {
        string RootPath { get; }

        IFileInfo GetFileInfo(string path);

        DirectoryInfo GetDirectoryInfo(string path);

        IEnumerable<IFileInfo> ListFiles(string path);

        IEnumerable<DirectoryInfo> ListDirectories(string path);

        string Combine(params string[] paths);

        Task CreateFileAsync(string path, string content);

        Stream CreateFile(string path);

        Task<string> ReadFileAsync(string path);

        Stream OpenFile(string path);

        void StoreFile(string sourceFileName, string destinationPath);

        void DeleteFile(string path);

        DateTimeOffset GetFileLastWriteTimeUtc(string path);

        void CreateDirectory(string path);

        bool DirectoryExists(string path);

        string MapPath(string path);
    }
}