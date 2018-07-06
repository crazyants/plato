using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Entities.Stores
{

    public class EntityStore : IEntityStore<Entity>
    {
     
        #region "Constructor"

        private readonly ICacheManager _cacheManager;
        private readonly IEntityRepository<Entity> _entityRepository;
        private readonly IEntityDataStore<EntityData> _entityDataStore;
        private readonly ILogger<EntityStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
     
        public EntityStore(
            ITypedModuleProvider typedModuleProvider,
            IEntityRepository<Entity> entityRepository,
            ILogger<EntityStore> logger,
            IDbQueryConfiguration dbQuery, 
            ICacheManager cacheManager,
            IEntityDataStore<EntityData> entityDataStore)
        {
            _typedModuleProvider = typedModuleProvider;
            _entityRepository = entityRepository;
            _cacheManager = cacheManager;
            _entityDataStore = entityDataStore;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<Entity> CreateAsync(Entity entity)
        {
            
            var data = new List<EntityData>();
            foreach (var item in entity.MetaData)
            {
                data.Add(new EntityData()
                {
                    Key = item.Key.FullName,
                    Value = item.Value.Serialize()
                });
            }
            entity.Data = data;
            
            var newEntity = await _entityRepository.InsertUpdateAsync(entity);
            if (newEntity != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                newEntity = await GetByIdAsync(newEntity.Id);
            }
            
            return newEntity;

        }

        public async Task<Entity> UpdateAsync(Entity entity)
        {
            var output = await _entityRepository.InsertUpdateAsync(entity);
            if (output != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }
            return output;
        }

        public async Task<bool> DeleteAsync(Entity entity)
        {
         
            var success = await _entityRepository.DeleteAsync(entity.Id);
            if (success)
            {
                _cacheManager.CancelTokens(this.GetType());
            }
            
            return success;
        }

        public async Task<Entity> GetByIdAsync(int id)
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var entity = await _entityRepository.SelectByIdAsync(id);
                return await MergeEntityData(entity);
            });

        }

        public IQuery<Entity> QueryAsync()
        {
            var query = new EntityQuery(this);
            return _dbQuery.ConfigureQuery<Entity>(query); ;
        }
      
        public async Task<IPagedResults<Entity>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await _entityRepository.SelectAsync(args);
                if (results != null)
                {
                    results.Data = await MergeEntityData(results.Data);
                }
                return results;
            });
        }

        #endregion

        #region "Private Methods"


        async Task<IList<Entity>> MergeEntityData(IList<Entity> entities)
        {

            if (entities == null)
            {
                return null;
            }

            // Get all entity data matching supplied entity ids
            var results = await _entityDataStore.QueryAsync()
                .Select<EntityDataQueryParams>(q => { q.EntityId.IsIn(entities.Select(e => e.Id).ToArray()); })
                .ToList();
            
            // Merge data into entities
            return await MergeEntityData(entities, results.Data);
            
        }

        async Task<IList<Entity>> MergeEntityData(IList<Entity> entities, IList<EntityData> data)
        {

            if (entities == null || data == null)
            {
                return entities;
            }

            for (var i = 0; i < entities.Count; i++)
            {
                entities[i].Data = data.Where(d => d.EntityId == entities[i].Id).ToList();
                entities[i] = await MergeEntityData(entities[i]);
            }

            return entities;

        }

        async Task<Entity> MergeEntityData(Entity entity)
        {

            if (entity == null)
            {
                return null;
            }

            if (entity.Data == null)
            {
                return entity;
            }

            foreach (var data in entity.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    entity.SetMetaData(type, (ISerializable) obj);
                }
            }

            return entity;

        }
        
        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }
     
        #endregion

    }

}
