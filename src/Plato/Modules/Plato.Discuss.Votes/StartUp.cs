using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Ratings.Services;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Discuss.Models;
using Plato.Discuss.Votes.Navigation;
using Plato.Discuss.Votes.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Votes
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

            // Register navigation providers
            services.AddScoped<INavigationProvider, TopicDetailsMenu>();
            services.AddScoped<INavigationProvider, TopicReplyDetailsMenu>();

            // Entity rating aggregator
            services.AddScoped<IEntityRatingAggregator<Topic>, EntityRatingAggregator<Topic>>();
            services.AddScoped<IEntityReplyRatingAggregator<Reply>, EntityReplyRatingAggregator<Reply>>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DiscussVotesWebApi",
                areaName: "Plato.Discuss.Votes",
                template: "api/discuss/{controller}/{action}/{id?}",
                defaults: new { controller = "Vote", action = "Get" }
            );
            
        }

    }

}