using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Cache.Abstractions;

namespace Plato.Internal.Cache
{

    public class CacheDependency : ICacheDependency
    {
        #region "Private Variables"

        private readonly ConcurrentDictionary<string, CacheDependencyInfo> _dependencies;

        #endregion

        #region "Constructor"

        public CacheDependency()
        {
            _dependencies = new ConcurrentDictionary<string, CacheDependencyInfo>();
        }

        #endregion

        #region "Implementation"

        public IChangeToken GetToken(string key)
        {
            return _dependencies.GetOrAdd(key, _ =>
            {
                var cancellationToken = new CancellationTokenSource();
                var changeToken = new CancellationChangeToken(cancellationToken.Token);
                return new CacheDependencyInfo()
                {
                    ChangeToken = changeToken,
                    CancellationToken = cancellationToken
                };
            }).ChangeToken;
        }

        public void CancelToken(string key)
        {
            if (_dependencies.TryRemove(key, out CacheDependencyInfo changeTokenInfo))
            {
                changeTokenInfo.CancellationToken.Cancel();
            }
        }

        #endregion

    }
    
}