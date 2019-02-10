using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Users.StopForumSpam.Stores;
using Plato.Users.StopForumSpam.Models;
using Plato.Users.StopForumSpam.Navigation;
using Plato.Users.StopForumSpam.ViewProviders;

namespace Plato.Users.StopForumSpam
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

            // Stores
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