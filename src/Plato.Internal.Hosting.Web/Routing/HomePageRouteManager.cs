using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Routing;

namespace Plato.Internal.Hosting.Web.Routing
{
 
    public class HomeRouteManager : IHomeRouteManager
    {

        private IEnumerable<HomeRoute> _routes;

        private readonly IEnumerable<IHomeRouteProvider> _providers;
        private readonly ILogger<HomeRouteManager> _logger;

        public HomeRouteManager(
            IEnumerable<IHomeRouteProvider> providers,
            ILogger<HomeRouteManager> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<HomeRoute> GetDefaultRoutes()
        {

            if (_routes == null)
            {
                var routes = new List<HomeRoute>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        foreach (var route in provider.GetRoutes())
                        {
                            routes.Add(route);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the homepage route provider '{provider.GetType()}'. Please review your reputation provider and try again. {e.Message}");
                        throw;
                    }
                }

                _routes = routes;
            }

            return _routes;

        }

    }

}
