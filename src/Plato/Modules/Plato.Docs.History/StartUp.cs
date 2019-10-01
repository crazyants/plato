using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Docs.History.Navigation;
using Plato.Docs.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Entities.History.Subscribers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Docs.History.Handlers;

namespace Plato.Docs.History
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Doc>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<DocComment>>();
          
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DocHistory",
                areaName: "Plato.Docs.History",
                template: "docs/history/{id:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DocRollbackHistory",
                areaName: "Plato.Docs.History",
                template: "docs/history/rollback",
                defaults: new { controller = "Home", action = "Rollback" }
            );

            routes.MapAreaRoute(
                name: "DocDeleteHistory",
                areaName: "Plato.Docs.History",
                template: "docs/history/delete",
                defaults: new { controller = "Home", action = "Delete" }
            );

        }

    }

}