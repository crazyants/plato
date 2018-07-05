using System;
using System.Collections.Generic;
using System.Text;
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


        public Task<EntityData> CreateAsync(EntityData model)
        {
            throw new NotImplementedException();
        }

        public Task<EntityData> UpdateAsync(EntityData model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(EntityData model)
        {
            throw new NotImplementedException();
        }

        public Task<EntityData> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
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
    }
}
