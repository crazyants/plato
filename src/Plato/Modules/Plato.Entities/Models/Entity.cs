using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Entities.Models
{
    public class Entity : EntityBase
    {
        
        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;


        public int FeatureId { get; set; }
        
        public string Title { get; set; }

        public string TitleNormalized { get; set; }
        
        public IList<EntityData> Data { get; set; } = new List<EntityData>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;
        
        public Entity()
        {
            _metaData = new ConcurrentDictionary<Type, ISerializable>();
        }
        
        public void AddOrUpdate<T>(T obj) where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                _metaData.TryUpdate(typeof(T), (ISerializable) obj, _metaData[typeof(T)]);
            }
            else
            {
                _metaData.TryAdd(typeof(T), (ISerializable)obj);
            }
        }

        public void AddOrUpdate(Type type, ISerializable obj)
        {
            if (_metaData.ContainsKey(type))
            {
                _metaData.TryUpdate(type, (ISerializable)obj, _metaData[type]);
            }
            else
            {
                _metaData.TryAdd(type, obj);
            }
        }

        public T TryGet<T>() where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                return (T)_metaData[typeof(T)];
            }

            return null;

        }
        
        public override void PopulateModel(IDataReader dr)
        {

            base.PopulateModel(dr);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);
            
            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("TitleNormalized"))
                TitleNormalized = Convert.ToString(dr["TitleNormalized"]);

        }

    }
    
}
