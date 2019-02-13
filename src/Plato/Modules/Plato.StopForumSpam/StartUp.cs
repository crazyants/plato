using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Stores;
using Plato.StopForumSpam.Navigation;
using Plato.StopForumSpam.ViewProviders;

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
            
            // Operations type manager
            services.AddScoped<ISpamOperationManager<SpamOperation>, SpamOperationManager<SpamOperation>>();

            // Spam level checker
            services.AddScoped<ISpamChecker, SpamChecker>();
            
            // Settings store
            services.AddScoped<IStopForumSpamSettingsStore<StopForumSpamSettings>, StopForumSpamSettingsStore>();

            // Admin view provider
            services.AddScoped<IViewProviderManager<StopForumSpamSettings>, ViewProviderManager<StopForumSpamSettings>>();
            services.AddScoped<IViewProvider<StopForumSpamSettings>, AdminViewProvider>();

            // Navigation provider
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