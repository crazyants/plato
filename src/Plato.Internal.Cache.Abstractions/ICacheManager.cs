using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Plato.Internal.Cache.Abstractions
{

    public interface ICacheManager
    {

        CacheToken GetOrCreateToken(Type type, params object[] varyBy);
        
        Task<TItem> GetOrCreateAsync<TItem>(CacheToken token, Func<ICacheEntry, Task<TItem>> factory);
        
        void CancelToken(CacheToken token);

        void CancelTokens(Type type);

        void CancelTokens(Type type, params object[] varyBy);
    }

}
