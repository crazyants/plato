using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Hosting.Web.Routing
{
    public class PlatoRouterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, RequestDelegate> _pipelines = new Dictionary<string, RequestDelegate>();
        private readonly ILogger _logger;

        public PlatoRouterMiddleware(
            RequestDelegate next,
            ILogger<PlatoRouterMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Begin Routing Request");
            }
                
            var shellSettings = (ShellSettings) httpContext.Features[typeof(ShellSettings)];

            // Define a PathBase for the current request that is the RequestUrlPrefix.
            // This will allow any view to reference ~/ as the tenant's base url.
            // Because IIS or another middleware might have already set it, we just append the tenant prefix value.
            if (!String.IsNullOrEmpty(shellSettings.RequestedUrlPrefix))
            {
                httpContext.Request.PathBase += ("/" + shellSettings.RequestedUrlPrefix);
                httpContext.Request.Path = httpContext.Request.Path.ToString()
                    .Substring(httpContext.Request.PathBase.Value.Length);
            }

            // Do we need to rebuild the pipeline ?
            var rebuildPipeline = httpContext.Items["BuildPipeline"] != null;
            lock (_pipelines)
            {
                if (rebuildPipeline && _pipelines.ContainsKey(shellSettings.Name))
                {
                    _pipelines.Remove(shellSettings.Name);
                }
            }
            
            RequestDelegate pipeline;

            // Building a pipeline can't be done by two requests
            lock (_pipelines)
            {
                if (!_pipelines.TryGetValue(shellSettings.Name, out pipeline))
                {
                    pipeline = BuildTenantPipeline(shellSettings, httpContext);
                    if (shellSettings.State == TenantState.Running)
                    {
                        _pipelines.Add(shellSettings.Name, pipeline);
                    }
                }
            }
            
            await pipeline.Invoke(httpContext);

        }

        // Build the middleware pipeline for the current tenant
        public RequestDelegate BuildTenantPipeline(
            ShellSettings shellSettings, 
            HttpContext httpContext)
        {

            var serviceProvider = httpContext.RequestServices;
            var startUps = serviceProvider.GetServices<IStartup>();
            var inlineConstraintResolver = serviceProvider.GetService<IInlineConstraintResolver>();
            var appBuilder = new ApplicationBuilder(serviceProvider);

            var routePrefix = "";
            if (!string.IsNullOrWhiteSpace(shellSettings.RequestedUrlPrefix))
                routePrefix = shellSettings.RequestedUrlPrefix + "/";
            
            var routeBuilder = new RouteBuilder(appBuilder)
            {
                DefaultHandler = serviceProvider.GetRequiredService<MvcRouteHandler>()
            };

            // Build prefixed route builder
            var prefixedRouteBuilder = new PrefixedRouteBuilder(
                routePrefix, 
                routeBuilder,
                inlineConstraintResolver);

            // Configure modules
            foreach (var startup in startUps)
            {
                startup.Configure(appBuilder, prefixedRouteBuilder, serviceProvider);
            }
                 
            //// Add the default template route to each shell 
            prefixedRouteBuilder.Routes.Add(new Route(
                prefixedRouteBuilder.DefaultHandler,
                "Default",
                "{area:exists}/{controller}/{action}/{id?}",
                null,
                null,
                null,
                inlineConstraintResolver)
            );

            routeBuilder.Routes.Insert(0, AttributeRouting.CreateAttributeMegaRoute(serviceProvider));

            // Attempt to get homepage route for tenant from site settings store
            // If the tenant has not been created yet siteService will return null
            // if siteService returns null users will be presented with the SetUp module
            var siteService = routeBuilder.ServiceProvider.GetService<ISiteSettingsStore>();
            if (siteService != null)
            {
                // Add home page route
                routeBuilder.Routes.Add(new HomePageRoute(
                    shellSettings.RequestedUrlPrefix,
                    siteService,
                    routeBuilder,
                    inlineConstraintResolver));
            }
            
            // Build router
            var router = prefixedRouteBuilder.Build();

            // Use router
            appBuilder.UseRouter(router);
            
            // Create a captured HttpContext for use outside of application context
            var capturedHttpContext = serviceProvider.GetService<ICapturedHttpContext>();
            capturedHttpContext.Configure(state => { state.Contextualize(httpContext); });

            // Create a captured router for use outside of application context
            var capturedRouter = serviceProvider.GetService<ICapturedRouter>();
            capturedRouter.Configure(options =>
            {
                options.Router = router;
                options.BaseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
            });
        
            // Activate all registered message broker subscriptions
            var subscribers = serviceProvider.GetServices<IBrokerSubscriber>();
            foreach (var subscriber in subscribers)
            {
                subscriber?.Subscribe();
            }

            // Activate all background tasks 
            var backgroundTaskManager = serviceProvider.GetService<IBackgroundTaskManager>();
            backgroundTaskManager?.StartTasks();

            // Return new pipeline
            var pipeline = appBuilder.Build();
            return pipeline;
        }

    }
    
}
