using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;

namespace Plato.Internal.Cache
{

    /// <summary>
    /// A simple implementation with no consideration for multi threaded environments.
    /// Mainly used when we want to simplify the cache for debugging purposes. 
    /// </summary>
    public class SimpleCacheManager : ICacheManager
    {

        public static ConcurrentDictionary<CacheToken, Type> Tokens { get; } =
       new ConcurrentDictionary<CacheToken, Type>();
        
        private readonly ICacheDependency _cacheDependency;
        private readonly ILogger<CacheManager> _logger;
        private readonly IMemoryCache _memoryCache;

        public SimpleCacheManager(ICacheDependency cacheDependency,
            IMemoryCache memoryCache,
            ILogger<CacheManager> logger)
        {
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(CacheToken token, Func<ICacheEntry, Task<TItem>> factory)
        {

            var key = token.ToString();

            // Item does not exist in cache
            if (!_memoryCache.TryGetValue(key, out var obj))
            {

                // Create ICacheEntry
                var entry = _memoryCache.CreateEntry(key);

                // Invoke our func delegate
                obj = (object)await factory(entry);

                // Set expiration tokens
                entry.ExpirationTokens.Add(_cacheDependency.GetToken(key));


                // Set value
                entry.SetValue(obj);

                // need to manually call dispose instead of having a using
                // statement in case the factory passed in throws, in which case we
                // do not want to add the entry to the cache
                entry.Dispose();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var type = obj != null
                        ? ((TItem)obj).GetType()
                        : typeof(TItem);
                    _logger.LogInformation("Added entry to cache of type {0} with key: {1}",
                        type.Name, key);
                }

            }

            return (TItem)obj;

        }

        public CacheToken GetOrCreateToken(Type type, params object[] varyBy)
        {
            var cacheToken = new CacheToken(type, varyBy);
            if (Tokens.ContainsKey(cacheToken))
            {
                return Tokens.FirstOrDefault(t => t.Key == cacheToken).Key;
            }

            Tokens.TryAdd(cacheToken, type);
            return cacheToken;
        }

        public void CancelTokens(Type type)
        {
            var tokens = GetTokensForType(type);
            foreach (var token in tokens)
            {
                CancelToken(token);
            }
        }

        public void CancelTokens(Type type, params object[] varyBy)
        {
            var cancellationToken = new CacheToken(type, varyBy);
            var tokens = GetTokensForType(type);
            foreach (var token in tokens)
            {
                if (cancellationToken == token)
                {
                    CancelToken(token);
                }

            }
        }

        public void CancelToken(CacheToken token)
        {
            if (Tokens.ContainsKey(token))
            {
                Tokens.TryRemove(token, out Type type);
            }

            _cacheDependency.CancelToken(token.ToString());

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Invalidated cache entry with key '{0}'",
                    token.ToString());
            }

        }

        IEnumerable<CacheToken> GetTokensForType(Type type)
        {
            return Tokens
                .Where(t => t.Value == type)
                .Select(c => c.Key)
                .ToList();
        }


    }

}
