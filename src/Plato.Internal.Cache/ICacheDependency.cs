using Microsoft.Extensions.Primitives;

namespace Plato.Internal.Cache
{

    public interface ICacheDependency
    {
        IChangeToken GetToken(string key);

        void CancelToken(string key);

    }

}
