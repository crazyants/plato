using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityParticipantsStore<TModel> : IStore<TModel> where TModel : class
    {
    }

    public class EntityParticipantsStore : IEntityParticipantsStore<EntityParticipant>
    {

        private string _key = "EntityParticipants";
        
        private readonly IEntityParticipantsRepository<EntityParticipant> _entityParticipantsRepository;
        private readonly ILogger<EntityParticipantsStore> _logger;
        private readonly ICacheDependency _cacheDependency;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQuery _dbQuery;

        public EntityParticipantsStore(
            IEntityParticipantsRepository<EntityParticipant> entityParticipantsRepository,
            ILogger<EntityParticipantsStore> logger,
            ICacheDependency cacheDependency,
            IMemoryCache memoryCache,
            IDbQuery dbQuery)
        {
            _entityParticipantsRepository = entityParticipantsRepository;
            _logger = logger;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
            _dbQuery = dbQuery;
        }

        public async Task<EntityParticipant> CreateAsync(EntityParticipant participant)
        {

            var newEntity = await _entityParticipantsRepository.InsertUpdateAsync(participant);
            if (newEntity != null)
            {
                _cacheDependency.CancelToken(GetEntityParticipantCacheKey());
            }

            return newEntity;

        }

        public async Task<EntityParticipant> UpdateAsync(EntityParticipant participant)
        {

            var newEntity = await _entityParticipantsRepository.InsertUpdateAsync(participant);
            if (newEntity != null)
            {
                _cacheDependency.CancelToken(GetEntityParticipantCacheKey());
            }

            return newEntity;

        }

        public async Task<bool> DeleteAsync(EntityParticipant participant)
        {
            var success = await _entityParticipantsRepository.DeleteAsync(participant.Id);
            if (success)
            {
                _cacheDependency.CancelToken(GetEntityParticipantCacheKey());
            }

            return success;

        }

        public async Task<EntityParticipant> GetByIdAsync(int id)
        {
            return await _entityParticipantsRepository.SelectByIdAsync(id);
        }

        public IQuery QueryAsync()
        {
            var query = new EntityParticipantQuery(this);
            return _dbQuery.ConfigureQuery(query);
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            var key = GetEntityParticipantCacheKey();
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var output = await _entityParticipantsRepository.SelectAsync<T>(args);
                if (output != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                }

                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return output;
            });

        }

        public async Task<IPagedResults<EntityParticipant>> SelectAsync(params object[] args)
        {
            var key = GetEntityParticipantCacheKey();
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var output = await _entityParticipantsRepository.SelectAsync<EntityParticipant>(args);
                if (output != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                }

                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return output;
            });

        }

        string GetEntityParticipantCacheKey()
        {
            return $"{_key}";
        }


    }

}
