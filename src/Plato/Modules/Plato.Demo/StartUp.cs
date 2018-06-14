using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Shell.Abstractions;
using Plato.Demo.Services;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;

namespace Plato.Demo
{
    public class Startup : StartupBase
    {
        private readonly ShellSettings _shellSettings;

        public Startup(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}