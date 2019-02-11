using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.StopForumSpam
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            //// StopForumSpam services
            services.AddScoped<IStopForumSpamClient, StopForumSpamClient>();
            services.AddScoped<IStopForumSpamChecker, StopForumSpamChecker>();

            // Operations manager
            services.AddScoped<ISpamOperationsManager<SpamOperation>, SpamOperationsManager<SpamOperation>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}