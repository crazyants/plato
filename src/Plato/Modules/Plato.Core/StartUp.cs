using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Shell.Abstractions;
using Plato.Core.Services;
using Plato.Internal.Features;
using Plato.Internal.Models.Shell;

namespace Plato.Core
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

            // set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}