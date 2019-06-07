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

            // Ensure all shells are loaded and available.
            _platoHost.Initialize();

            // Get ShellSettings for current tennet
            var shellSettings = _runningShellTable.Match(httpContext);
            
            // register shell settings as a custom feature
            httpContext.Features[typeof(ShellSettings)] = shellSettings;

            // only serve the next request if the tenant has been resolved.
            if (shellSettings != null)
            {

                var shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
                var hasPendingTasks = false;
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

                    var deferredTaskEngine = scope.ServiceProvider.GetService<IDeferredTaskManager>();
                    if (deferredTaskEngine == null)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("No service has been found for type IDeferredTaskManager. Deferred tasks cannot execute.");
                        }
                    }

                    hasPendingTasks = deferredTaskEngine?.HasPendingTasks ?? false;

                }

                // Create a new scope only if there are pending tasks
                if (hasPendingTasks)
                {
                    shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
                    using (var pendingScope = shellContext.CreateServiceScope())
                    {
                        if (pendingScope != null)
                        {
                            var deferredTaskEngine = pendingScope.ServiceProvider.GetService<IDeferredTaskManager>();
                            var context = new DeferredTaskContext(pendingScope.ServiceProvider);
                            await deferredTaskEngine.ExecuteTaskAsync(context);
                        }
                    }
                }

            }

        }

    }
}
