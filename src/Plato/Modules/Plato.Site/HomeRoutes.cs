using System.Collections.Generic;
using Plato.Internal.Abstractions.Routing;

namespace Plato.Site
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Site", "Home", "Index")
            };
        }

    }

}
