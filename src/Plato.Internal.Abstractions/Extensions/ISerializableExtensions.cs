using System;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class SerializableExtensions
    {
   
        public static async Task<T> DeserializeGenericTypeAsync<T>(
            this ISerializable serializable, string data) where T : class
        {
            return await ((ISerializable)ActivateInstanceOf<T>.Instance()).DeserializeAsync<T>(data);
        }

    }
}
