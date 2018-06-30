using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Discuss.Stores
{

    public interface ITopicStore<T> : IStore<T> where T : class
    {

    }

    public class TopicStore : ITopicStore<Topic>
    {

        private readonly IEntityStore<Entity> _entityStore;

        public TopicStore(
            IEntityStore<Entity> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<Topic> CreateAsync(Topic topic)
        {
            
            var data = new List<EntityData>();
            foreach (var item in topic.MetaData)
            {
                data.Add(new EntityData()
                {
                    Key = item.Key.FullName,
                    Value = item.Value.Serialize()
                });
            }

            topic.Data = data;
            
            var entity = await _entityStore.CreateAsync(topic);
            if (entity != null)
            {
                return await GetByIdAsync(entity.Id);
            }

            return null;

        }

        public Task<bool> DeleteAsync(Topic model)
        {
            throw new NotImplementedException();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {

            Topic topic = null;
            var entity = await _entityStore.GetByIdAsync(id);
            if (entity != null)
            {

                var asm = typeof(Topic).Assembly;

                topic = new Topic();
                foreach (var data in entity.Data)
                {

                    var type = asm.GetType(data.Key);
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    dynamic invokedType = null;
                    if (constructor != null)
                    {
                        invokedType = constructor.Invoke(null);
                    }
                    
                    if (invokedType is ISerializable serializable)
                    {
                        var value = await invokedType.DeserializeAsync<dynamic>(data.Value);
                        ISerializable o = TConverter.ChangeType(type, value);
                        topic.MetaData.Add(type, o);
                       
                    }

                }

            }
            
            return topic;

        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<Topic> UpdateAsync(Topic model)
        {
            throw new NotImplementedException();
        }
    }


    public static class TConverter
    {
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(typeof(T), value);
        }

        public static object ChangeType(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }

        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {

            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
    }

}
