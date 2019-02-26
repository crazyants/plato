using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Security;
using Plato.Internal.Security.Abstractions;
using Plato.Moderation.Models;
using Plato.Moderation.Handlers;
using Plato.Moderation.Repositories;
using Plato.Moderation.Services;
using Plato.Moderation.Stores;

namespace Plato.Moderation
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
            services.AddScoped<IModeratorRepository<Moderator>, ModeratorRepository>();

            // Stores
            services.AddScoped<IModeratorStore<Moderator>, ModeratorStore>();
            
            // Register moderator permissions manager
            services.AddScoped<IPermissionsManager<ModeratorPermission>, PermissionsManager<ModeratorPermission>>();
          
            // Register additional authorization handler for implied permissions
            services.AddScoped<IAuthorizationHandler, ModeratorPermissionsHandler>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

        }

    }
}