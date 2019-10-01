using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Ideas.History.Navigation;
using Plato.Ideas.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Entities.History.Subscribers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Ideas.History.Handlers;

namespace Plato.Ideas.History
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
            services.AddScoped<INavigationProvider, DocCommentMenu>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Idea>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<IdeaComment>>();
          
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "IdeaHistory",
                areaName: "Plato.Ideas.History",
                template: "ideas/history/{id:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "IdeaRollbackHistory",
                areaName: "Plato.Ideas.History",
                template: "ideas/history/rollback",
                defaults: new { controller = "Home", action = "Rollback" }
            );

            routes.MapAreaRoute(
                name: "IdeaDeleteHistory",
                areaName: "Plato.Ideas.History",
                template: "ideas/history/delete",
                defaults: new { controller = "Home", action = "Delete" }
            );

        }

    }

}