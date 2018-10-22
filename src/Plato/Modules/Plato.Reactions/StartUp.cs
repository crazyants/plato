using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Reactions.Handlers;
using Plato.Reactions.Models;
using Plato.Reactions.Repositories;
using Plato.Reactions.Services;
using Plato.Reactions.Stores;

namespace Plato.Reactions
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
            services.AddScoped<IEntityReactionRepository<EntityReacttion>, EntityEntityReactionRepository>();

            // Stores
            services.AddScoped<IReactionStore<EntityReacttion>, ReactionStore>();

            // Services
            services.AddScoped<IReactionsManager<Reaction>, ReactionsManager<Reaction>>();

            // Managers
            services.AddScoped<IEntityReactionsManager, EntityEntityReactionsesManager>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}