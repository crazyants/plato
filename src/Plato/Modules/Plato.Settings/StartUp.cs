using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Hosting;
using Plato.Shell.Models;
using Plato.Abstractions.SetUp;
using Plato.Settings.Services;

namespace Plato.Settings
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
           
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}