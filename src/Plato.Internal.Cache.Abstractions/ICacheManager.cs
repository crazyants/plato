using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Plato.Internal.Cache.Abstractions
{

    public interface ICacheManager
    {

        Task<TItem> GetOrCreateAsync<TItem>(CacheToken token, Func<ICacheEntry, Task<TItem>> factory);

        CacheToken GetOrCreateToken(Type type, params object[] varyBy);

        void CancelToken(CacheToken token);

        void CancelTokens(Type type);

        void CancelTokens(Type type, params object[] varyBy);
    }

}
