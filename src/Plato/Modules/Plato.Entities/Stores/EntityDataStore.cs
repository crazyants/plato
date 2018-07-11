using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityDataStore<T> : IStore<T> where T : class
    {

        Task<IEnumerable<T>> GetByEntityIdAsync(int entityId);

    }

    public class EntityDataStore : IEntityDataStore<EntityData>
    {

        private readonly ICacheManager _cacheManager;
        private readonly IEntityDataRepository<EntityData> _entityDataRepository;
        private readonly ILogger<EntityDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
        
        public EntityDataStore(
            ICacheManager cacheManager,
            IEntityDataRepository<EntityData> entityDataRepository, 
            ILogger<EntityDataStore> logger,
            IDbQueryConfiguration dbQuery,
            ITypedModuleProvider typedModuleProvider)
        {
            _cacheManager = cacheManager;
            _entityDataRepository = entityDataRepository;
            _logger = logger;
            _dbQuery = dbQuery;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public async Task<EntityData> CreateAsync(EntityData model)
        {
            var result =  await _entityDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<EntityData> UpdateAsync(EntityData model)
        {
            var result = await _entityDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityData model)
        {
            var success = await _entityDataRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted eneity data with key '{0}' for entity id {1}",
                        model.Key, model.EntityId);
                }

                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectByIdAsync(id));
        }

        public IQuery<EntityData> QueryAsync()
        {
            var query = new EntityDataQuery(this);
            return _dbQuery.ConfigureQuery<EntityData>(query); ;
        }

        public async Task<IPagedResults<EntityData>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectAsync(args));
        }

        public async Task<IEnumerable<EntityData>> GetByEntityIdAsync(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectByEntityIdAsync(entityId));
        }

    }

}
