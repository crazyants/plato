using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Entities.Stores
{

    public class EntityStore<TEntity> : IEntityStore<TEntity> where TEntity : class, IEntity
    {

        public const string  ById = "ById";
        public const string ByFeatureId = "ByFeatureId";
        
        private readonly IEntityDataStore<IEntityData> _entityDataStore;
        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<EntityStore<TEntity>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        
        public EntityStore(
            IEntityDataStore<IEntityData> entityDataStore,
            IEntityRepository<TEntity> entityRepository,
            ITypedModuleProvider typedModuleProvider,
            ILogger<EntityStore<TEntity>> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _typedModuleProvider = typedModuleProvider;
            _entityRepository = entityRepository;
            _entityDataStore = entityDataStore;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<TEntity> CreateAsync(TEntity model)
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

        public async Task<TEntity> UpdateAsync(TEntity model)
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
                    _logger.LogInformation("Updated entity with id {1}",
                        updatedEntity.Id);
                }

                CancelTokens(updatedEntity);

            }

            return updatedEntity;

        }

        public async Task<bool> DeleteAsync(TEntity model)
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

        public async Task<TEntity> GetByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var entity = await _entityRepository.SelectByIdAsync(id);
                return await MergeEntityData(entity);
            });

        }

        public IQuery<TEntity> QueryAsync()
        {
            var query = new EntityQuery<TEntity>(this);
            return _dbQuery.ConfigureQuery<TEntity>(query); ;
        }

        public async Task<IPagedResults<TEntity>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await _entityRepository.SelectAsync(args);
                if (results != null)
                {
                    results.Data = await MergeEntityData(results.Data);
                    //results.Data = PrepareHierarchy(results.Data.ToLookup(c => c.ParentId));
                    //results.Data = results.Data.OrderBy(r => r.SortOrder).ToList();
                }
                return results;
            });

        }
        
        public async Task<IEnumerable<TEntity>> GetByFeatureIdAsync(int featureId)
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByFeatureId, featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await _entityRepository.SelectByFeatureIdAsync(featureId);
                if (results != null)
                {
                    results = await MergeEntityData(results.ToList());
                    //results = PrepareHierarchy(results.ToLookup(c => c.ParentId));
                    //results = results.OrderBy(r => r.SortOrder);
                }

                return results;

            });

        }

        public async Task<IEnumerable<TEntity>> GetParentsByIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var entity = await GetByIdAsync(entityId);
            if (entity == null)
            {
                return null;
            }

            if (entity.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entity.FeatureId));
            }

            var entities = await GetByFeatureIdAsync(entity.FeatureId);
            return entities?.BuildHierarchy<TEntity>()
                .RecurseParents<TEntity>(entity.Id).Reverse();

            //return RecurseParents(entities.ToList(), entity.Id).Reverse();

        }

        public async Task<IEnumerable<TEntity>> GetChildrenByIdAsync(int entityId)
        {

            var entity = await GetByIdAsync(entityId);
            if (entity == null)
            {
                return null;
            }

            var entities = await GetByFeatureIdAsync(entity.FeatureId);
            return entities?.BuildHierarchy<TEntity>()
                .RecurseChildren<TEntity>(entity.Id).Reverse();

            //return RecurseChildren(entities.ToList(), entity.Id);

        }

        #endregion

        #region "Private Methods"

        //////IList<TEntity> PrepareHierarchy(
        //////    ILookup<int, TEntity> input,
        //////    IList<TEntity> output = null,
        //////    TEntity parent = null,
        //////    int parentId = 0,
        //////    int depth = 0)
        //////{

        //////    if (input == null) throw new ArgumentNullException(nameof(input));
        //////    if (output == null) output = new List<TEntity>();
        //////    if (parentId == 0) depth = 0;

        //////    foreach (var item in input[parentId])
        //////    {

        //////        if (depth < 0) depth = 0;
        //////        if (parent != null) depth++;

        //////        item.Depth = depth;
        //////        item.Parent = parent;
                
        //////        if (parent != null)
        //////        {
        //////            var children = new List<IEntity>() { item };
        //////            if (parent.Children != null)
        //////            {
        //////                children.AddRange(parent.Children);
        //////            }

        //////            parent.Children = children.OrderBy(c => c.SortOrder);
        //////        }

        //////        output.Add(item);

        //////        // recurse
        //////        PrepareHierarchy(input, output, item, item.Id, depth--);
        //////    }

        //////    return output;

        //////}


        //////IEnumerable<TEntity> RecurseParents(
        //////    IList<TEntity> input,
        //////    int rootId,
        //////    IList<TEntity> output = null)
        //////{
        //////    if (output == null)
        //////    {
        //////        output = new List<TEntity>();
        //////    }

        //////    foreach (var item in input)
        //////    {
        //////        if (item.Id == rootId)
        //////        {
        //////            if (item.ParentId > 0)
        //////            {
        //////                output.Add(item);
        //////                RecurseParents(input, item.ParentId, output);
        //////            }
        //////            else
        //////            {
        //////                output.Add(item);
        //////            }
        //////        }
        //////    }

        //////    return output;

        //////}

        //////IEnumerable<TEntity> RecurseChildren(
        //////    IList<TEntity> input,
        //////    int rootId,
        //////    IList<TEntity> output = null)
        //////{

        //////    if (output == null)
        //////    {
        //////        output = new List<TEntity>();
        //////    }

        //////    foreach (var item in input)
        //////    {
        //////        if (item.ParentId == rootId)
        //////        {
        //////            output.Add(item);
        //////            RecurseChildren(input, item.Id, output);
        //////        }
        //////    }

        //////    return output;

        //////}

        async Task<IEnumerable<IEntityData>> SerializeMetaDataAsync(TEntity entity)
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

        async Task<IList<TEntity>> MergeEntityData(IList<TEntity> entities)
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

        async Task<IList<TEntity>> MergeEntityData(IList<TEntity> entities, IList<IEntityData> data)
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

        async Task<TEntity> MergeEntityData(TEntity entity)
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

        void CancelTokens(TEntity model)
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
