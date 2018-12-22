using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Plato.Internal.FileSystem.Abstractions;

namespace Plato.Internal.FileSystem.Uploads
{
    public class PhysicalUploadFolder : IUploadFolder
    {

        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<PhysicalUploadFolder> _logger;
 
        private const int MaxFileNameLength = 32;

        private static string InternalRootPath = "wwwroot/uploads";

        public PhysicalUploadFolder(
            IPlatoFileSystem parentFileSystem,
            ILogger<PhysicalUploadFolder> logger)
        {
            _logger = logger;

            if (!parentFileSystem.DirectoryExists(InternalRootPath))
            {
                parentFileSystem.CreateDirectory(InternalRootPath);
            }

            var root = parentFileSystem.GetDirectoryInfo(InternalRootPath).FullName;
            _fileSystem = new PlatoFileSystem(root, new PhysicalFileProvider(root), _logger);

        }

        // Saves a unique file and returns the file name
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

            //var extension = System.IO.File.getfi fileName.
            string fullPath;

            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }

            do
            {
                fileName = System.Guid.NewGuid().ToString();
                fileName = fileName.Substring(0, MaxFileNameLength - extension.Length - 1);
                fileName = fileName + "." + extension;
                fullPath = path + fileName;
            } while (_fileSystem.FileExists(fullPath));

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Attrmpting to save unique file to {fullPath}.");
            }

            // write the file 
            await _fileSystem.CreateFileAsync(fullPath, stream);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Successfully saveed unique file at {fullPath}.");
            }


            return fileName;
            
        }

    }


}
