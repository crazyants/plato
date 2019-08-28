using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Docs.Reactions.Handlers;
using Plato.Docs.Reactions.Navigation;
using Plato.Docs.Reactions.Badges;
using Plato.Docs.Reactions.Tasks;

namespace Plato.Docs.Reactions
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, DocMenu>();
            services.AddScoped<INavigationProvider, DocFooterMenu>();
            services.AddScoped<INavigationProvider, DocCommentMenu>();
            services.AddScoped<INavigationProvider, DocCommentFooterMenu>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
                    
            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, ReactionBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, ReactionBadgesAwarder>();

            // Reaction providers
            services.AddScoped<IReactionsProvider<Reaction>, Reactions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}