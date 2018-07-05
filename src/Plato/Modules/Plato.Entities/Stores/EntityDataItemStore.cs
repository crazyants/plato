using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Entities.Stores
{
    public class EntityDataItemStore<T> : IEntityDataItemStore<T> where T : class
    {

        #region "Private Variables"

        private readonly IEntityDataRepository<EntityData> _entityDataRepository;

        #endregion

        #region ""Constructor"

        public EntityDataItemStore(
            IEntityDataRepository<EntityData> entityDataRepository)
        {
            _entityDataRepository = entityDataRepository;
        }

        #endregion

        #region "IMplementation"

        public async Task<T> GetAsync(int entityId, string key)
        {
            var result = await GetByEntityIdAndKeyAsync(entityId, key);
            if (result != null)
            {
                // If the generic type implements ISerializable use
                // the the DeserializeAsync method on the generic type
                // Allows us to implement our own Serialize and Deserialize
                // methods on the type if we want to override the default
                // Serializable base class virtual methods
                var serializable = typeof(T) as ISerializable;
                if (serializable != null)
                {
                    return await serializable.DeserializeGenericTypeAsync<T>(result.Value);
                }
                return await result.Value.DeserializeAsync<T>();
            }

            return default(T);
        }

        public async Task<T> UpdateAsync(int entityId, string key, ISerializable value)
        {

            // Get or create setting
            var data = await GetByEntityIdAndKeyAsync(entityId, key)
                       ?? new EntityData()
                       {
                           EntityId = entityId,
                           Key = key
                       };

            // Serilize value
            data.Value = await value.SerializeAsync();

            var updatedData = await _entityDataRepository.InsertUpdateAsync(data);
            if (updatedData != null)
            {
                return await GetAsync(entityId, updatedData.Key);
            }

            return default(T);
        }

        public Task<bool> DeleteAsync(int userId, string key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task<EntityData> GetByEntityIdAndKeyAsync(int userId, string key)
        {
            var allKeys = await GetAllKeys(userId);
            return allKeys?.FirstOrDefault(s => s.Key == key);
        }

        private async Task<IEnumerable<EntityData>> GetAllKeys(int entityId)
        {
            var output = new List<EntityData>();
            var entries = await _entityDataRepository.SelectDataByEntityId(entityId);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    output.Add(entry);
                }
            }

            return output;
        }
        
        #endregion

    }

}
