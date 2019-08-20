using System.Collections.Generic;
using Plato.Internal.Abstractions.Routing;

namespace Plato.Core
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Core", "Home", "Index")
            };
        }

    }

}
