using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Shell;
using Plato.Internal.Shell.Models;
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

            // Get ShellSettings for current tennet
            var shellSettings = _runningShellTable.Match(httpContext);
            
            // register shell settings as a custom feature
            httpContext.Features[typeof(ShellSettings)] = shellSettings;

            // only serve the next request if the tenant has been resolved.
            if (shellSettings != null)
            {

                var shellContext = _platoHost.GetOrCreateShellContext(shellSettings);
                using (var scope = shellContext.CreateServiceScope())
                {
                    httpContext.RequestServices = scope.ServiceProvider;
                    
                    if (!shellContext.IsActivated)
                    {
                        lock (shellSettings)
                        {
                            // activate the tanant
                            if (!shellContext.IsActivated)
                            {

                                //var tenantEvents = httpContext.RequestServices.GetServices<IModularTenantEvents>();
                                
                                // BuildPipeline ensures we always rebuild routes for new tennets
                                httpContext.Items["BuildPipeline"] = true;
                                shellContext.IsActivated = true;
                            }
                        }
                    }

                    await _next.Invoke(httpContext);
                }

            }

        }

    }
}
