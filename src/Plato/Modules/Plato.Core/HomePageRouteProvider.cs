using System.Collections.Generic;
using Plato.Internal.Abstractions.Routing;

namespace Plato.Core
{

    public class HomeRouteProvider : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Core", "Home", "Index"),
                new HomeRoute("Plato.Users", "Account", "Login"),
                new HomeRoute("Plato.Discuss", "Home", "Index"),
                new HomeRoute("Plato.Docs", "Home", "Index"),
                new HomeRoute("Plato.Articles", "Home", "Index"),
                new HomeRoute("Plato.Questions", "Home", "Index"),
                new HomeRoute("Plato.Ideas", "Home", "Index"),
                new HomeRoute("Plato.Issues", "Home", "Index")
            };
        }

    }

}
