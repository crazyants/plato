using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Cache
{
    
    public class CacheManager : ICacheManager
    {

        public static IDictionary<CacheToken, Type> Tokens { get; } = new Dictionary<CacheToken, Type>();

        private readonly IMemoryCache _memoryCache;
        private readonly ICacheDependency _cacheDependency;
        private readonly ILogger<CacheManager> _logger;

        public CacheManager(ICacheDependency cacheDependency,
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
                obj = (object) await factory(entry);

                // Set expiration token
                entry.ExpirationTokens.Add(_cacheDependency.GetToken(key));

                entry.SetValue(obj);
                entry.Dispose();
                
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added entry to cache of type {0} with key: {1}",
                        ((TItem)obj).GetType(), key);
                }
                
            }

            return (TItem)obj;

        }

        public void CancelTokens(Type type)
        {
            var tokens = GetTokensForType(type);
            foreach (var token in tokens)
            {
                CancelToken(token);
            }
        }

        public void CancelToken(CacheToken token)
        {
            Tokens.Remove(token);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Cancelling cache entry with key '{1}'", 
                   token.ToString());
            }
            _cacheDependency.CancelToken(token.ToString());
        }

        public CacheToken GetOrCreateToken(Type type, params object[] varyBy)
        {
            var key = new CacheToken(type, varyBy);
            if (Tokens.ContainsKey(key))
            {
                return key;
            }

            Tokens.Add(key, type);
            return key;
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
