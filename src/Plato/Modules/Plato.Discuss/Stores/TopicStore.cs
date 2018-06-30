using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Discuss.Stores
{

    //public interface ITopicStore<T> : IStore<T> where T : class
    //{

    //}

    //public class TopicStore : ITopicStore<Topic>
    //{

    //    private readonly IEntityStore<Entity> _entityStore;

    //    public TopicStore(
    //        IEntityStore<Entity> entityStore)
    //    {
    //        _entityStore = entityStore;
    //    }

    //    public async Task<Topic> CreateAsync(Topic topic)
    //    {
            
    //        var data = new List<EntityData>();
    //        foreach (var item in topic.MetaData)
    //        {
    //            data.Add(new EntityData()
    //            {
    //                Key = item.Key.AssemblyQualifiedName,
    //                Value = item.Value.Serialize()
    //            });
    //        }

    //        topic.Data = data;
            
    //        var entity = await _entityStore.CreateAsync(topic);
    //        if (entity != null)
    //        {
    //            return await GetByIdAsync(entity.Id);
    //        }

    //        return null;

    //    }

    //    public Task<bool> DeleteAsync(Topic model)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<Topic> GetByIdAsync(int id)
    //    {
     
    //        var entity = await _entityStore.GetByIdAsync<Topic>(id);
    //        if (entity != null)
    //        {
              
    //            foreach (var data in entity.Data)
    //            {
    //                // Ensure we can convert to a valid type
    //                var type = Type.GetType(data.Key);
    //                if (type != null)
    //                {
    //                    var obj = JsonConvert.DeserializeObject(data.Value, type);
    //                    entity.MetaData.Add(type, (ISerializable)obj);
    //                }
    //            }

    //        }
            
    //        return entity;

    //    }

    //    public IQuery QueryAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<Topic> UpdateAsync(Topic model)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


}
