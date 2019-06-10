using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Hosting.Abstractions;
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

            // Ensure all tenants are loaded and available.
            _platoHost.Initialize();

            // Get ShellSettings for current tenant
            var shellSettings = _runningShellTable.Match(httpContext);
            
            // Register shell settings as a custom feature
            httpContext.Features[typeof(ShellSettings)] = shellSettings;

            // Only serve the next request if the tenant has been resolved.
            if (shellSettings != null)
            {

                var hasDeferredTasks = false;
                var shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
                using (var scope = shellContext.CreateServiceScope())
                {

                    // Mimic the services provided by our host and tenant
                    httpContext.RequestServices = scope.ServiceProvider;

                    // Ensure the tenant is activated
                    if (!shellContext.IsActivated)
                    {
                        lock (shellSettings)
                        {
                            // Activate the tenant
                            if (!shellContext.IsActivated)
                            {

                                // BuildPipeline ensures we always rebuild routes for new tenants
                                httpContext.Items["BuildPipeline"] = true;
                                shellContext.IsActivated = true;
                                
                                // Activate message broker subscriptions
                                var subscribers = scope.ServiceProvider.GetServices<IBrokerSubscriber>();
                                foreach (var subscriber in subscribers)
                                {
                                    subscriber.Subscribe();
                                }

                                // Activate background tasks 
                                var backgroundTaskManager = scope.ServiceProvider.GetService<IBackgroundTaskManager>();
                                backgroundTaskManager?.StartTasks();

                            }
                        }
                    }

                    // Invoke the next middleware in pipeline
                    await _next.Invoke(httpContext);

                    // At the end determine if we need to process deferred tasks
                    var deferredTaskManager = scope.ServiceProvider.GetService<IDeferredTaskManager>();
                    hasDeferredTasks = deferredTaskManager?.Process(httpContext) ?? false;
                    
                }
                
                // Execute deferred tasks (if any) within a new scope
                // once the entire response has been sent to the client.
                if (hasDeferredTasks)
                {
                    httpContext.Response.OnCompleted(async () =>
                    {
                        using (var scope = shellContext.CreateServiceScope())
                        {
                            var context = new DeferredTaskContext(scope.ServiceProvider);
                            var deferredTaskManager = scope.ServiceProvider.GetService<IDeferredTaskManager>();
                            await deferredTaskManager.ExecuteTaskAsync(context);
                        }
                    });
                }
                
            }

        }

    }
}
