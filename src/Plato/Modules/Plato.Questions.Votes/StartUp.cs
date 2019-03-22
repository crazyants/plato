using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Models;
using Plato.Questions.Votes.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Models;
using Plato.Questions.Votes.Navigation;
using Plato.Entities.Ratings.Subscribers;

namespace Plato.Questions.Votes
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
            services.AddScoped<INavigationProvider, QuestionDetailsMenu>();
            services.AddScoped<INavigationProvider, AnswerDetailsMenu>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntityRatingSubscriber<Question>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}