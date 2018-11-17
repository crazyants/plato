using System;
using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models
{

    public interface IMetaData<TModel> where TModel : class
    {
        IDictionary<Type, ISerializable> MetaData { get; }

        IEnumerable<TModel> Data { get; set; }

        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

    }
    
}
