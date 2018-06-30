using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
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

        private readonly IEntityRepository<Entity> _entityRepository;
        private readonly ILogger<EntityStore> _logger;
        private readonly ICacheDependency _cacheDependency;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQuery _dbQuery;
        private readonly IModuleManager _moduleManager;

        public EntityStore(
            ILogger<EntityStore> logger,
            IEntityRepository<Entity> entityRepository,
            ICacheDependency cacheDependency,
            IMemoryCache memoryCache,
            IDbQuery dbQuery, 
            IModuleManager moduleManager)
        {
            _logger = logger;
            _entityRepository = entityRepository;
            _dbQuery = dbQuery;
            _moduleManager = moduleManager;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
        }

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
                _cacheDependency.CancelToken(GetEntityCacheKey());
                newEntity = await GetByIdAsync(newEntity.Id);
            }
            
            return newEntity;

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
            var entity = await _entityRepository.SelectByIdAsync(id);
            if (entity != null)
            {
                foreach (var data in entity.Data)
                {
                    var type = await GetModuleTypeCandidateAsync(data.Key);
                    if (type != null)
                    {
                        var obj = JsonConvert.DeserializeObject(data.Value, type);
                        entity.SetMetaData((ISerializable)obj, type);
                    }
                }
            }

            return entity;
        }


        private async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            
            // Loop through each module assembly
            foreach (var assembly in await _moduleManager.LoadModuleAssembliesAsync())
            {
                // Get all classes that implement ISerializable
                foreach (var candidate in assembly.ExportedTypes
                    .Where(p => typeof(ISerializable).IsAssignableFrom(p))
                    .Select(t => t.GetTypeInfo()))
                {
                    if (candidate.GetTypeInfo().FullName == typeName)
                    {
                        return candidate.AsType();
                    }
                }
            }

            return null;

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
                var output = await _entityRepository.SelectAsync<T>(args);
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


        private string GetEntityCacheKey()
        {
            return $"{_key}";
        }


     
    }

}
