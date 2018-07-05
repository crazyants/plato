using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
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
     
        private string _key = "Entity";

        #region "Constructor"

        private readonly ICacheManager _cacheManager;

        private readonly IEntityRepository<Entity> _entityRepository;
        private readonly ILogger<EntityStore> _logger;
        //private readonly ICacheDependency _cacheDependency;
        //private readonly IMemoryCache _memoryCache;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
        //private readonly IEntityDataStore<EntityData> _entityDataStore;

        public EntityStore(
            ITypedModuleProvider typedModuleProvider,
            IEntityRepository<Entity> entityRepository,
            ICacheDependency cacheDependency,
            ILogger<EntityStore> logger,
            IMemoryCache memoryCache,
            IDbQueryConfiguration dbQuery, 
            IEntityDataStore<EntityData> entityDataStore,
            ICacheManager cacheManager)
        {
            _typedModuleProvider = typedModuleProvider;
            _entityRepository = entityRepository;
            //_cacheDependency = cacheDependency;
            //_memoryCache = memoryCache;
            _dbQuery = dbQuery;
            //_entityDataStore = entityDataStore;
            _cacheManager = cacheManager;
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
            
            // Add entity
            var newEntity = await _entityRepository.InsertUpdateAsync(entity);
            if (newEntity != null)
            {
                _cacheManager.CancelTokens(this.GetType());

                //_cacheDependency.CancelToken(GetEntityCacheKey());
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

                //_cacheDependency.CancelToken(GetEntityCacheKey());
            }
            return output;
        }

        public async Task<bool> DeleteAsync(Entity entity)
        {
         
            var success = await _entityRepository.DeleteAsync(entity.Id);
            if (success)
            {

                _cacheManager.CancelTokens(this.GetType());

                //_cacheDependency.CancelToken(cacheKey.ToString());
            }
            
            return success;
        }

        public async Task<Entity> GetByIdAsync(int id)
        {
            
            var entity = await _entityRepository.SelectByIdAsync(id);
            if (entity != null)
            {
                foreach (var data in entity.Data)
                {
                    var type = await GetModuleTypeCandidateAsync(data.Key);
                    if (type != null)
                    {
                        var obj = JsonConvert.DeserializeObject(data.Value, type);
                        entity.SetMetaData(type, (ISerializable)obj);
                    }
                }
            }

            return entity;
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
                var output = await _entityRepository.SelectAsync(args);
                if (output != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Adding entry to cache with key: {0}",
                            token.ToString());
                    }
                }
                //cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(token.ToString()));
                return output;
            });

        }

        #endregion

        #region "Private Methods"
        
        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }
        
        private string GetEntityCacheKey()
        {
            return $"{_key}";
        }
        private string GetEntityCacheKey(int hashCode)
        {
            return $"{_key}_{hashCode}";
        }
        
        #endregion

    }

}
