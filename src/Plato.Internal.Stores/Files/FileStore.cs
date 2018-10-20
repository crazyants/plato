using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Stores.Abstractions.Files;

namespace Plato.Internal.Stores.Files
{
    
    public class FileStore : IFileStore
    {

        private readonly IMemoryCache _memoryCache;
        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<FileStore> _logger;
      
        public FileStore(
            IMemoryCache memoryCache,
            IPlatoFileSystem fileSystem,
            ILogger<FileStore> logger)
        {
            _memoryCache = memoryCache;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public async Task<byte[]> GetFileBytesAsync(string path)
        {

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .AddExpirationToken(_fileSystem.Watch(path));

            if (!_memoryCache.TryGetValue(path, out byte[] result))
            {
                result = await _fileSystem.ReadFileBytesAsync(path);
                if (result != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation(
                            $"Adding entry to cache of type {_memoryCache.GetType().Name}. Entry key: {path}.");
                    }
                 
                    _memoryCache.Set(path, result, cacheEntryOptions);
                }
            }

            return result;
        }
        
        public async Task<string> GetFileAsync(string path)
        {

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .AddExpirationToken(_fileSystem.Watch(path));

            if (!_memoryCache.TryGetValue(path, out string result))
            {
                result = await _fileSystem.ReadFileAsync(path);
                if (result != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, path);
                    _memoryCache.Set(path, result, cacheEntryOptions);
                }
            }

            return result;
        }

        public string Combine(params string[] paths)
        {
            return _fileSystem.Combine(paths);
        }

    }

}