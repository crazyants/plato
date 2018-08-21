using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Moderation.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Discuss.Moderation.Navigation;
using Plato.Discuss.Moderation.Stores;
using Plato.Discuss.Moderation.ViewProviders;

namespace Plato.Discuss.Moderation
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
            
            // Data stores
            services.AddScoped<IModeratorStore<ModeratorDocument>, ModeratorStore<ModeratorDocument>>();
            
            // Admin view providers
            services.AddScoped<IViewProviderManager<Moderator>, ViewProviderManager<Moderator>>();
            services.AddScoped<IViewProvider<Moderator>, AdminViewProvider>();

            // Register navigation provider
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