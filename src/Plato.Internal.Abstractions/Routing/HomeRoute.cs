using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Abstractions.Routing
{

    public class HomeRoute : RouteValueDictionary
    {
        
        public HomeRoute()
        {
            this["area"] = "Plato.Core";
            this["controller"] = "Home";
            this["action"] = "Index";
        }

        public HomeRoute(string area) 
        {
            this["area"] = area;
        }

        public HomeRoute(string area, string controller) : this(area)
        {
            this["controller"] = controller;
        }

        public HomeRoute(string area, string controller, string action) : this(area, controller)
        {
            this["action"] = action;
        }

        public string Id => $"{this["area"]}/{this["controller"]}/{this["action"]}";
    }

}
