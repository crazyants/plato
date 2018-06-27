using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Stores
{

    public class EntityStore : IEntityStore<Entity>
    {

        private string _key = "Entity";

        private readonly IEntityRepository<Entity> _entityRepository;
        private readonly ILogger<EntityStore> _logger;
        private readonly ICacheDependency _cacheDependency;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQuery _dbQuery;

        public EntityStore(
            ILogger<EntityStore> logger,
            IEntityRepository<Entity> entityRepository,
            ICacheDependency cacheDependency,
            IMemoryCache memoryCache,
            IDbQuery dbQuery)
        {
            _logger = logger;
            _entityRepository = entityRepository;
            _dbQuery = dbQuery;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
        }

        public async Task<Entity> CreateAsync(Entity entity)
        {

            var output = await _entityRepository.InsertUpdateAsync(entity);
            if (output != null)
            {
                _cacheDependency.CancelToken(GetEntityCacheKey());
            }

            return output;

        }

        public async Task<Entity> UpdateAsync(Entity entity)
        {
            var output = await _entityRepository.InsertUpdateAsync(entity);

            if (output != null)
            {
                _cacheDependency.CancelToken(GetEntityCacheKey());
            }

            return output;

        }

        public async Task<bool> DeleteAsync(Entity entity)
        {
            var success = await _entityRepository.DeleteAsync(entity.Id);
            if (success)
            {
                _cacheDependency.CancelToken(GetEntityCacheKey());
            }

            return success;
        }

        public async Task<Entity> GetByIdAsync(int id)
        {
            return await _entityRepository.SelectByIdAsync(id);
        }

        public IQuery QueryAsync()
        {
            var query = new EntityQuery(this);
            return _dbQuery.ConfigureQuery(query); ;
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {

            var key = GetEntityCacheKey();
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var roles = await _entityRepository.SelectAsync<T>(args);
                if (roles != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                }
                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return roles;
            });

        }


        private string GetEntityCacheKey()
        {
            return $"{_key}";
        }



    }

}
