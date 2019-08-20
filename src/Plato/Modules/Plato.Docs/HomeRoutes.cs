using System.Collections.Generic;
using Plato.Internal.Abstractions.Routing;

namespace Plato.Docs
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Docs", "Home", "Index")
            };
        }

    }

}
