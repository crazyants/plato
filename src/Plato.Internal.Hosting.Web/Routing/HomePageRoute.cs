using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Internal.Hosting.Web.Routing
{
    public class HomePageRoute : Route
    {

        private readonly IRouteBuilder _routeBuilder;

        private RouteValueDictionary _tokens;
        private readonly ISiteSettingsStore _siteSettings;

        public HomePageRoute(
            string prefix, 
            ISiteSettingsStore siteSettings, 
            IRouteBuilder routeBuilder, 
            IInlineConstraintResolver inlineConstraintResolver)
            : base(routeBuilder.DefaultHandler, prefix ?? "", inlineConstraintResolver)
        {
            _siteSettings = siteSettings;
            _routeBuilder = routeBuilder;
        }

        protected override async Task OnRouteMatched(RouteContext context)
        {

            var siteSettings = await _siteSettings.GetAsync();

            // We may have settings but no homepage route specified
            // In this instance to avoid a 404 use default homepage route
            if (siteSettings != null && siteSettings.HomeRoute == null)
            {
                siteSettings.HomeRoute = new DefaultHomePageRoute();
            }

            // Use specified homepage route
            if (siteSettings?.HomeRoute != null)
            {
                foreach (var entry in siteSettings.HomeRoute)
                {
                    context.RouteData.Values[entry.Key] = entry.Value;
                }
                _tokens = siteSettings.HomeRoute;
            }
         

            await base.OnRouteMatched(context);

        }

        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            if (_tokens == null)
                return null;

            // Return null if it doesn't match the home route values
            foreach (var entry in _tokens)
            {
                object value;
                if (String.Equals(entry.Key, "area", StringComparison.OrdinalIgnoreCase))
                {
                    if (!context.AmbientValues.TryGetValue("area", out value) || !String.Equals(value.ToString(), _tokens["area"].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
                else
                {
                    if (!context.Values.TryGetValue(entry.Key, out value) || !String.Equals(value.ToString(), entry.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
            }
            
            // Remove the values that should not be rendered in the queryResult string
            foreach (var key in _tokens.Keys)
            {
                context.Values.Remove(key);
            }

            var result = base.GetVirtualPath(context);
            return result;

        }
    }
}
