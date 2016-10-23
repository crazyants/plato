using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Shell;
using Plato.Shell.Models;
using Plato.Hosting.Web.Extensions;


namespace Plato.Hosting.Web.Middleware
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

            // Ensure all ShellContext are loaded and available.
            _platoHost.Initialize();

            var shellSetting = _runningShellTable.Match(httpContext);

            // Register the shell settings as a custom feature.
            httpContext.Features[typeof(ShellSettings)] = shellSetting;

            // We only serve the next request if the tenant has been resolved.
            if (shellSetting != null)
            {
                ShellContext shellContext = _platoHost.GetOrCreateShellContext(shellSetting);

                using (var scope = shellContext.CreateServiceScope())
                {
                    httpContext.RequestServices = scope.ServiceProvider;

                    if (!shellContext.IsActivated)
                    {
                        lock (shellSetting)
                        {
                            // The tenant gets activated here
                            if (!shellContext.IsActivated)
                            {
                                //var eventBus = scope.ServiceProvider.GetService<IEventBus>();
                                //eventBus.NotifyAsync<IOrchardShellEvents>(x => x.ActivatingAsync()).Wait();
                                //eventBus.NotifyAsync<IOrchardShellEvents>(x => x.ActivatedAsync()).Wait();

                                shellContext.IsActivated = true;
                            }
                        }
                    }

                    await _next.Invoke(httpContext);
                }



                //using (var scope = shellContext.CreateServiceScope())
                //{
                //    var deferredTaskEngine = scope.ServiceProvider.GetService<IDeferredTaskEngine>();

                //    if (deferredTaskEngine != null && deferredTaskEngine.HasPendingTasks)
                //    {
                //        var context = new DeferredTaskContext(scope.ServiceProvider);
                //        await deferredTaskEngine.ExecuteTasksAsync(context);
                //    }
                //}
            }

            await _next.Invoke(httpContext);

        }
    }
}
