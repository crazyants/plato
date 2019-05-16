using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Ratings.Services;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Issues.Models;
using Plato.Issues.Votes.Handlers;
using Plato.Issues.Votes.Navigation;

namespace Plato.Issues.Votes
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
            services.AddScoped<INavigationProvider, IssueDetailsMenu>();
            services.AddScoped<INavigationProvider, IssueCommentDetailsMenu>();

            // Entity rating aggregator
            services.AddScoped<IEntityRatingAggregator<Issue>, EntityRatingAggregator<Issue>>();
            services.AddScoped<IEntityReplyRatingAggregator<Comment>, EntityReplyRatingAggregator<Comment>>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "IssueVotesWebApi",
                areaName: "Plato.Issues.Votes",
                template: "api/issues/{controller}/{action}/{id?}",
                defaults: new { controller = "Vote", action = "Get" }
            );


        }
    }
}