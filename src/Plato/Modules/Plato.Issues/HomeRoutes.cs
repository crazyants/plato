using System.Collections.Generic;
using Plato.Internal.Abstractions.Routing;

namespace Plato.Issues
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Issues", "Home", "Index")
            };
        }

    }

}
