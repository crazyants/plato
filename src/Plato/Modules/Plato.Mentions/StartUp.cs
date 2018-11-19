using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Assets;
using Plato.Mentions.Handlers;
using Plato.Mentions.Models;
using Plato.Mentions.Repositories;
using Plato.Mentions.Services;
using Plato.Mentions.Stores;
using Plato.Mentions.Subscribers;

namespace Plato.Mentions
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

            // Repositories
            services.AddScoped<IEntityMentionsRepository<EntityMention>, EntityMentionsRepository>();

            // Stores
            services.AddScoped<IEntityMentionsStore<EntityMention>, EntityMentionsStore>();

            // Parsers
            services.AddScoped<IMentionsParser, MentionsParser>();

            // Register broker subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();
            
            // Managers
            services.AddScoped<IEntityMentionsManager<EntityMention>, EntityMentionsManager>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}