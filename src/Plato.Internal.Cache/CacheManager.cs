using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Cache.Abstractions;

namespace Plato.Internal.Cache
{
    
    public class CacheManager : ICacheManager
    {

        public const int ExpirationInSeconds = 120;
        
        private bool UseDistributedCache => !string.IsNullOrEmpty(_config["ConnectionStrings:Redis"]);
        
        public static ConcurrentDictionary<CacheToken, Type> Tokens { get; } = new ConcurrentDictionary<CacheToken, Type>();

        private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockers = new ConcurrentDictionary<string, SemaphoreSlim>();
        
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheDependency _cacheDependency;
        private readonly ILogger<CacheManager> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;

        public CacheManager(
            IDistributedCache distributedCache,
            ICacheDependency cacheDependency,
            ILogger<CacheManager> logger,
            IMemoryCache memoryCache,
            IConfiguration config)
        {
            _distributedCache = distributedCache;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
            _config = config;
            _logger = logger;
        }


        public async  Task<TItem> GetOrCreateAsync<TItem>(CacheToken token, Func<ICacheEntry, Task<TItem>> factory)
        {
            
            // Generate unique key
            var key = token.ToString();

            // Validate key
            ValidateKey(key);

            // Guaranteed single callback execution for multi threaded request for the same key. 
            object cacheItem = new AsyncLazy<TItem>(async () =>
            {

                var fromCache = GetFromCache<TItem>(key);
                if (fromCache != null) return fromCache;

                var locker = _lockers.GetOrAdd(key, new SemaphoreSlim(1, 1));
                try
                {
                    await locker.WaitAsync().ConfigureAwait(false);

                    fromCache = GetFromCache<TItem>(key);
                    if (fromCache != null) return fromCache;

                    // No cache entry found, invoke entry & factory delegate
                    var entry = _memoryCache.CreateEntry(key);

                    // Invoke factory delegate
                    var obj = await factory(entry);
                    
                    // Write result to cache
                    await WriteToCache(entry, key, obj);
                    
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        var type = obj != null
                            ? ((TItem)obj).GetType()
                            : typeof(TItem);
                        _logger.LogInformation("Added entry to cache of type {0} with key: {1}",
                            type.Name, key);
                    }

                    return obj;
                }
                finally
                {
                    locker.Release();
                    _lockers.TryRemove(key, out var _);
                }

            });
            var result = UnwrapAsyncLazy<TItem>(cacheItem);
            return await result.ConfigureAwait(false);
        }
        
        public CacheToken GetOrCreateToken(Type type, params object[] varyBy)
        {

            var cacheToken = new CacheToken(type, varyBy);
            if (Tokens.ContainsKey(cacheToken))
            {
                // Equivalent to .First(t => t.Key == cacheToken).Key
                // LINQ is avoided for performance / allocation reasons
                foreach (var t in Tokens)
                {
                    if (t.Key == cacheToken)
                    {
                        return t.Key;
                    }
                }
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
                Tokens.TryRemove(token, out var type);
            }
          
            _cacheDependency.CancelToken(token.ToString());

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Invalidated cache entry with key '{0}'",
                    token.ToString());
            }

        }

        // ----------------
        
        T GetFromCache<T>(string key)
        {
            try
            {
                var cacheObject = _memoryCache.Get<T>(key);
                if (cacheObject != null) return cacheObject;

                if (!UseDistributedCache) return default(T);

                var byteCache = _distributedCache.Get(key);
                if (byteCache == null) return default(T);
                var content = Encoding.UTF8.GetString(byteCache);
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return default(T);
            }
        }

        async Task WriteToCache<T>(ICacheEntry entry, string key, T obj)
        {
            try
            {
                if (!UseDistributedCache)
                {
                    
                    entry.SetOptions(new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTime.UtcNow.AddSeconds(ExpirationInSeconds)
                    });

                    // Set expiration tokens
                    entry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                    entry.SetValue(obj);
                    // need to manually call dispose instead of having a using
                    // statement in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();

                    //_memoryCache.Set(entry.Key.ToString(), obj, DateTimeOffset.UtcNow.AddSeconds(expirationInSeconds));
                    return;
                }
                var toStore = JsonConvert.SerializeObject(obj);
                await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(toStore), new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(ExpirationInSeconds)
                });
            }
            catch
            {

                // DistributedCache storage failed. Fail over to MemoryCache.

                entry.SetOptions(new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(ExpirationInSeconds)
                });
                
                // Set expiration tokens
                entry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                entry.SetValue(obj);
                // need to manually call dispose instead of having a using
                // statement in case the factory passed in throws, in which case we
                // do not want to add the entry to the cache
                entry.Dispose();
                //_memoryCache.Set(key, obj, DateTimeOffset.UtcNow.AddSeconds(expirationInSeconds));
            }
        }
        
        Task<T> UnwrapAsyncLazy<T>(object item)
        {

            if (item is AsyncLazy<T> asyncLazy)
            {
                return asyncLazy.Value;
            }
            
            if (item is Task<T> task)
            {
                return task;
            }
            
            if (item is Lazy<T> lazy)
            {
                return Task.FromResult(lazy.Value);
            }
            
            if (item is T variable)
            {
                return Task.FromResult(variable);
            }

            return Task.FromResult(default(T));

        }

        void ValidateKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }


            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key), "Cache keys cannot be empty or whitespace");
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
