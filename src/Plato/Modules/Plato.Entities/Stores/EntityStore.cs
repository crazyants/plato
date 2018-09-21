using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Entities.Stores
{

    public class EntityStore<TEntitty> : IEntityStore<TEntitty> where TEntitty : class, IEntity
    {

        private readonly ICacheManager _cacheManager;
        private readonly IEntityRepository<TEntitty> _entityRepository;
        private readonly IEntityDataStore<IEntityData> _entityDataStore;
        private readonly ILogger<EntityStore<TEntitty>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public EntityStore(
            ITypedModuleProvider typedModuleProvider,
            IEntityRepository<TEntitty> entityRepository,
            ILogger<EntityStore<TEntitty>> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            IEntityDataStore<IEntityData> entityDataStore)
        {
            _typedModuleProvider = typedModuleProvider;
            _entityRepository = entityRepository;
            _cacheManager = cacheManager;
            _entityDataStore = entityDataStore;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<TEntitty> CreateAsync(TEntitty model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var newEntity = await _entityRepository.InsertUpdateAsync(model);
            if (newEntity != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added new entity with id {1}",
                        newEntity.Id);
                }

                CancelTokens(newEntity);
            }

            return newEntity;
        }

        public async Task<TEntitty> UpdateAsync(TEntitty model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var updatedEntity = await _entityRepository.InsertUpdateAsync(model);
            if (updatedEntity != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated existing entity with id {1}",
                        updatedEntity.Id);
                }

                CancelTokens(updatedEntity);

            }

            return updatedEntity;

        }

        public async Task<bool> DeleteAsync(TEntitty model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var success = await _entityRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity with id {1}", model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<TEntitty> GetByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var entity = await _entityRepository.SelectByIdAsync(id);
                return await MergeEntityData(entity);
            });

        }

        public IQuery<TEntitty> QueryAsync()
        {
            var query = new EntityQuery<TEntitty>(this);
            return _dbQuery.ConfigureQuery<TEntitty>(query); ;
        }

        public async Task<IPagedResults<TEntitty>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

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

        async Task<IEnumerable<IEntityData>> SerializeMetaDataAsync(TEntitty entity)
        {

            // Get all existing entity data
            var data = await _entityDataStore.GetByEntityIdAsync(entity.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<IEntityData>();

            // Iterate all meta data on the supplied type,
            // check if a key already exists, if so update existing key 
            var output = new List<IEntityData>();
            foreach (var item in entity.MetaData)
            {
                var key = item.Key.FullName;
                var entityData = dataList.FirstOrDefault(d => d.Key == key);
                if (entityData != null)
                {
                    entityData.Value = await item.Value.SerializeAsync();
                }
                else
                {
                    entityData = new EntityData()
                    {
                        Key = key,
                        Value = item.Value.Serialize()
                    };
                }

                output.Add(entityData);
            }

            return output;

        }

        async Task<IList<TEntitty>> MergeEntityData(IList<TEntitty> entities)
        {

            if (entities == null)
            {
                return null;
            }

            // Get all entity data matching supplied entity ids
            var results = await _entityDataStore.QueryAsync()
                .Select<EntityDataQueryParams>(q => { q.EntityId.IsIn(entities.Select(e => e.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return entities;
            }

            // Merge data into entities
            return await MergeEntityData(entities, results.Data);

        }

        async Task<IList<TEntitty>> MergeEntityData(IList<TEntitty> entities, IList<IEntityData> data)
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

        async Task<TEntitty> MergeEntityData(TEntitty entity)
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
                    entity.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return entity;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

        void CancelTokens(TEntitty model)
        {

            // Clear cache for current type, EntityStore<Entity>,
            // EntityStore<Topic>, EntityStore<Article> etc
            _cacheManager.CancelTokens(this.GetType());

            // If we instantiate the EntityStore via a derived type
            // of IEntity i.e. EntityStore<SomeEntity> ensures we clear
            // the cache for the base entity store. We don't want our
            // base entity cache polluting our derived type cache
            if (this.GetType() != typeof(EntityStore<Entity>))
            {
                _cacheManager.CancelTokens(typeof(EntityStore<Entity>));
            }

            // Clear entity data
            _cacheManager.CancelTokens(typeof(EntityDataStore));

        }

        #endregion

    }

}
