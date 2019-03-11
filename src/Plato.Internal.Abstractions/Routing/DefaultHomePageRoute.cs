using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Abstractions.Routing
{
    public class DefaultHomePageRoute : RouteValueDictionary
    {

        public DefaultHomePageRoute()
        {
            this["area"] = "Plato.Core";
            this["controller"] = "Home";
            this["action"] = "Index";
        }

    }

}
