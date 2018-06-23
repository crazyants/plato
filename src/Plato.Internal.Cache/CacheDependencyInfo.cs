using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Plato.Internal.Cache
{

    class CacheDependencyInfo
    {

        public IChangeToken ChangeToken { get; set; }

        public CancellationTokenSource CancellationToken { get; set; }

    }

}
