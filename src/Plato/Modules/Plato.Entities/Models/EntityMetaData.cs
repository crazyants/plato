using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Entities.Models
{

    public class EntityMetaData<T> where T : class
    {

        public T Data { get; set; }

        public EntityMetaData(T data)
        {
            Data = data;
        }

    }

    public static class Wrapper
    {
        public static EntityMetaData<T> Create<T>(T wrapped) where T : class
        {
            return new EntityMetaData<T>(wrapped);
        }

        public static EntityMetaData<T> Wrap<T>(this T wrapped) where T : class
        {
            return Create(wrapped);
        }
    }

}
