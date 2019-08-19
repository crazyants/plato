using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Abstractions.Routing
{
    public interface IHomeRouteProvider
    {

        IEnumerable<HomeRoute> GetRoutes();

    }
}
