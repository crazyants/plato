using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Abstractions.Routing
{
    public class DefaultHomePageRoute : RouteValueDictionary
    {

        public DefaultHomePageRoute()
        {
            this["Area"] = "Plato.Core";
            this["Controller"] = "Home";
            this["Action"] = "Index";
        }

    }

}
