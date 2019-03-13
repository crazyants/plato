using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Plato.Internal.FileSystem.Abstractions;

namespace Plato.Internal.FileSystem.Uploads
{
    public class PhysicalUploadFolder : IUploadFolder
    {
        
        private const int ByMaxFileNameLength = 32;
        
        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<PhysicalUploadFolder> _logger;

        public PhysicalUploadFolder(
            IPlatoFileSystem parentFileSystem,
            ILogger<PhysicalUploadFolder> logger)
        {
            _logger = logger;

            if (!parentFileSystem.DirectoryExists(Path))
            {
                parentFileSystem.CreateDirectory(Path);
            }

            var root = parentFileSystem.GetDirectoryInfo(Path).FullName;
            _fileSystem = new PlatoFileSystem(root, new PhysicalFileProvider(root), _logger);

        }
        
        public string Path => "wwwroot/uploads";

        public async Task<string> SaveUniqueFileAsync(
            Stream stream,
            string fileName, 
            string path)
        {
            
            var parts = fileName.Split('.');
            var extension = parts[parts.Length - 1];

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception("Could not obtain a fie extension!");
            }

            if (extension.Length > 6)
            {
                throw new Exception("The file extension is not valid!");
            }
            
            string fullPath;

            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }

            do
            {
                fileName = System.Guid.NewGuid().ToString();
                fileName = fileName.Substring(0, ByMaxFileNameLength - extension.Length - 1);
                fileName = fileName + "." + extension;
                fullPath = path + fileName;
            } while (_fileSystem.FileExists(fullPath));

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Attempting to save unique file to {fullPath}.");
            }

            // write the file 
            await _fileSystem.CreateFileAsync(fullPath, stream);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Successfully saved unique file at {fullPath}.");
            }
            
            return fileName;
            
        }

        public async Task<string> SaveFileAsync(Stream stream, string fileName, string path)
        {

            var parts = fileName.Split('.');
            var extension = parts[parts.Length - 1];

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception("Could not obtain a fie extension!");
            }

            if (extension.Length > 6)
            {
                throw new Exception("The file extension is not valid!");
            }
            
            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }

            var fullPath = path + fileName;

            // write the file 
            await _fileSystem.CreateFileAsync(fullPath, stream);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Successfully saved unique file at {fullPath}.");
            }

            return fileName;


        }

        public async Task CreateFileAsync(string path, Stream steam)
        {
            await _fileSystem.CreateFileAsync(path, steam);
        }

        public bool DeleteFile(string fileName, string path)
        {
            
            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }


            var fullPath = path + fileName;

            if (_fileSystem.FileExists(fullPath))
            {
                _fileSystem.DeleteFile(fullPath);
            }

            return false;
        }

    }


}
