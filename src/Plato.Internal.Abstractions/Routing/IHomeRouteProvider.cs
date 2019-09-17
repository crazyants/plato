using System.Collections.Generic;

namespace Plato.Internal.Abstractions.Routing
{
    public interface IHomeRouteProvider
    {
        IEnumerable<HomeRoute> GetRoutes();
    }

}
