using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityDataStore<T> : IStore<T> where T : class
    {

        Task<IEnumerable<T>> GetByEntityIdAsync(int entityId);

    }

    public class EntityDataStore : IEntityDataStore<IEntityData>
    {

        private readonly ICacheManager _cacheManager;
        private readonly IEntityDataRepository<IEntityData> _entityDataRepository;
        private readonly ILogger<EntityDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
        
        public EntityDataStore(
            ICacheManager cacheManager,
            IEntityDataRepository<IEntityData> entityDataRepository, 
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
        
        public async Task<IEntityData> CreateAsync(IEntityData model)
        {
            var result =  await _entityDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<IEntityData> UpdateAsync(IEntityData model)
        {
            var result = await _entityDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(IEntityData model)
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

        public async Task<IEntityData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectByIdAsync(id));
        }

        public IQuery<IEntityData> QueryAsync()
        {
            var query = new EntityDataQuery(this);
            return _dbQuery.ConfigureQuery<IEntityData>(query); ;
        }

        public async Task<IPagedResults<IEntityData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<IEntityData>> GetByEntityIdAsync(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectByEntityIdAsync(entityId));
        }

    }

}
