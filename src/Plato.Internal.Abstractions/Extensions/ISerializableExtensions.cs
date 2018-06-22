using System;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class SerializableExtensions
    {

        public static async Task<T> DeserializeGenericTypeAsync<T>(
            this ISerializable serializable, string data)
        {
            return ((T)await ((ISerializable)
                Activator.CreateInstance(typeof(T))).DeserializeAsync<T>(data));
        }

    }
}
