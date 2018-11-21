using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Discuss.Reactions.Handlers;
using Plato.Discuss.Reactions.Navigation;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Discuss.Reactions.Assets;
using Plato.Discuss.Reactions.Badges;

namespace Plato.Discuss.Reactions
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
            services.AddScoped<INavigationProvider, TopicMenu>();
            services.AddScoped<INavigationProvider, TopicFooterMenu>();
            services.AddScoped<INavigationProvider, TopicReplyMenu>();
            services.AddScoped<INavigationProvider, TopicReplyFooterMenu>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, ReactionBadges>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}