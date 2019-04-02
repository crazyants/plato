using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Primitives;

namespace Plato.Internal.FileSystem.Abstractions
{
    public interface IPlatoFileSystem
    {
        string RootPath { get; }

        IFileInfo GetFileInfo(string path);

        DirectoryInfo GetDirectoryInfo(string path);

        IEnumerable<IFileInfo> ListFiles(string path);

        IEnumerable<IFileInfo> ListFiles(string path, Matcher matcher);

        IEnumerable<DirectoryInfo> ListDirectories(string path);

        string Combine(params string[] paths);

        Task CreateFileAsync(string path, string content);

        Task SaveFileAsync(string path, string content);

        Task CreateFileAsync(string path, Stream stream);

        Stream CreateFile(string path);

        Task<string> ReadFileAsync(string path);

        Task<byte[]> ReadFileBytesAsync(string path);

        IChangeToken Watch(string path);

        Stream OpenFile(string path);

        void StoreFile(string sourceFileName, string destinationPath);

        void CopyDirectory(string sourceDirectory, string destinationDirectory, bool copyChildDirectories);

        bool DeleteFile(string path);

        bool DeleteDirectory(string path);

        DateTimeOffset GetFileLastWriteTimeUtc(string path);

        void CreateDirectory(string path);

        bool DirectoryExists(string path);

        bool FileExists(string path);

        string MapPath(string path);

    }

}