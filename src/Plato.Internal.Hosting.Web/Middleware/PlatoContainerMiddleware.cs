using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.TagHelpers;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Hosting.Web.Middleware
{
    public class PlatoContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPlatoHost _platoHost;
        private readonly IRunningShellTable _runningShellTable;
        private readonly ILogger _logger;

        public PlatoContainerMiddleware(
            RequestDelegate next,
            IPlatoHost platoHost,
            IRunningShellTable runningShellTable,
            ILogger<PlatoContainerMiddleware> logger)
        {
            _next = next;
            _platoHost = platoHost;
            _runningShellTable = runningShellTable;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            // Ensure all shells are loaded and available.
            _platoHost.Initialize();

            // Get ShellSettings for current tennet
            var shellSettings = _runningShellTable.Match(httpContext);
            
            // register shell settings as a custom feature
            httpContext.Features[typeof(ShellSettings)] = shellSettings;

            // only serve the next request if the tenant has been resolved.
            if (shellSettings != null)
            {

                var hasPendingTasks = false;
                var shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
                using (var scope = shellContext.CreateServiceScope())
                {
                    httpContext.RequestServices = scope.ServiceProvider;
                    
                    if (!shellContext.IsActivated)
                    {
                        lock (shellSettings)
                        {
                            // Activate the tanant
                            if (!shellContext.IsActivated)
                            {

                                // BuildPipeline ensures we always rebuild routes for new tennets
                                httpContext.Items["BuildPipeline"] = true;
                                shellContext.IsActivated = true;
                                
                                // Activate message broker subscriptions
                                var subscribers = scope.ServiceProvider.GetServices<IBrokerSubscriber>();
                                foreach (var subscriber in subscribers)
                                {
                                    subscriber.Subscribe();
                                }

                                // Activate tasks 
                                var backgroundTaskManager = scope.ServiceProvider.GetService<IBackgroundTaskManager>();
                                backgroundTaskManager?.StartTasks();

                            }
                        }
                    }
                    
                    await _next.Invoke(httpContext);

                    var deferredTaskStore = scope.ServiceProvider.GetService<IDeferredTaskStore>();
                    hasPendingTasks = deferredTaskStore?.Process(httpContext) ?? false;
                    
                }
                
                // ---------------

                if (hasPendingTasks)
                {
                    shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
                    using (var scope = shellContext.CreateServiceScope())
                    {
                        var deferredTaskStore = scope.ServiceProvider.GetService<IDeferredTaskStore>();
                        var context = new DeferredTaskContext(scope.ServiceProvider);
                        await deferredTaskStore.ExecuteTaskAsync(context);

                    }
                }

            }

        }

    }
}
