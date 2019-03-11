using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plato.Internal.Hosting.Web.Routing
{
    public class PrefixedRouteBuilder : IRouteBuilder
    {
        private readonly IRouteBuilder _baseRouteBuilder;
        private readonly List<IRouter> _routes = new List<IRouter>();
        private readonly string _routePrefix;
        private readonly IInlineConstraintResolver _constraintResolver;

        public PrefixedRouteBuilder(
            string routePrefix, 
            IRouteBuilder baseRouteBuilder,
            IInlineConstraintResolver constraintResolver)
        {
            _constraintResolver = constraintResolver;
            _routePrefix = routePrefix;
            _baseRouteBuilder = baseRouteBuilder;
        }

        public IApplicationBuilder ApplicationBuilder => _baseRouteBuilder.ApplicationBuilder;

        public IRouter DefaultHandler
        {
            get => _baseRouteBuilder.DefaultHandler;
            set => _baseRouteBuilder.DefaultHandler = value;
        }

        public IList<IRouter> Routes => _routes;

        public IServiceProvider ServiceProvider => _baseRouteBuilder.ServiceProvider;

        public IRouter Build()
        {
      
            foreach (var route in Routes.OfType<Route>())
            {
                var constraints = new Dictionary<string, object>();

                foreach (var kv in route.Constraints)
                {
                    constraints.Add(kv.Key, kv.Value);
                }

                var prefixedRoute = new Route(
                    _baseRouteBuilder.DefaultHandler,
                    route.Name,
                    _routePrefix + route.RouteTemplate,
                    route.Defaults,
                    constraints,
                    route.DataTokens,
                    _constraintResolver);

                _baseRouteBuilder.Routes.Add(prefixedRoute);
            }

            return _baseRouteBuilder.Build();
        }
    }
}
