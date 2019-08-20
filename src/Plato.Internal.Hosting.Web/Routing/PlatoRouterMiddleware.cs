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
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Settings;

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

        public async Task Invoke(HttpContext context)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Begin Routing Request");
            }
                
            var shellSettings = (ShellSettings)context.Features[typeof(ShellSettings)];

            // Define a PathBase for the current request that is the RequestUrlPrefix.
            // This will allow any view to reference ~/ as the tenant's base url.
            // Because IIS or another middleware might have already set it, we just append the tenant prefix value.
            if (!String.IsNullOrEmpty(shellSettings.RequestedUrlPrefix))
            {
                context.Request.PathBase += ("/" + shellSettings.RequestedUrlPrefix);
                context.Request.Path = context.Request.Path.ToString()
                    .Substring(context.Request.PathBase.Value.Length);
            }
            
            // Do we need to rebuild the pipeline ?
            var rebuildPipeline = context.Items["BuildPipeline"] != null;
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
                    pipeline = BuildTenantPipeline(shellSettings, context);
                    if (shellSettings.State == TenantState.Running)
                    {
                        _pipelines.Add(shellSettings.Name, pipeline);
                    }
                }
            }
            
            await pipeline.Invoke(context);
            
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
                "PlatoDefault",
                "{area:exists}/{controller}/{action}/{id?}",
                null,
                null,
                null,
                inlineConstraintResolver)
            );

            // Add attribute routing
            routeBuilder.Routes.Insert(0, AttributeRouting.CreateAttributeMegaRoute(serviceProvider));

            // TODO: The homepage route is not set via the Plato.Core module
            // Attempt to get homepage route for tenant from site settings store
            // If the tenant has not been created yet siteService will return null
            // if siteService returns null users will be presented with the SetUp module
            ////var siteService = routeBuilder.ServiceProvider.GetService<ISiteSettingsStore>();
            ////if (siteService != null)
            ////{
            
            ////    //// Add the default template route to each shell 
            ////    prefixedRouteBuilder.Routes.Add(new Route(
            ////        prefixedRouteBuilder.DefaultHandler,
            ////        "PlatoHome",
            ////        "",
            ////        new HomeRoute(),
            ////        null,
            ////        null,
            ////        inlineConstraintResolver)
            ////    );

            ////    // Add home page route matching
            ////    var homeRoute = new HomePageRoute(
            ////        shellSettings.RequestedUrlPrefix,
            ////        siteService,
            ////        routeBuilder,
            ////        inlineConstraintResolver);

            ////    routeBuilder.Routes.Add(homeRoute);
                
            ////}

            // ------------------

            // Build router
            var router = prefixedRouteBuilder.Build();

            // Use router
            appBuilder.UseRouter(router);

            // Create a captured HttpContext for use outside of application context
            var capturedHttpContext = serviceProvider.GetService<ICapturedHttpContext>();
            capturedHttpContext.Configure(state => state.Contextualize(httpContext));

            // Create a captured router for use outside of application context
            var capturedRouter = serviceProvider.GetService<ICapturedRouter>();
            capturedRouter.Configure(options =>
            {
                options.Router = router;
                options.BaseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
            });
        
            // Build & return new pipeline
            var pipeline = appBuilder.Build();
            return pipeline;

        }
        
    }

}
