using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Questions.Reactions.Handlers;
using Plato.Questions.Reactions.Navigation;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Questions.Reactions.Badges;
using Plato.Questions.Reactions.Tasks;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Questions.Reactions
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
            services.AddScoped<INavigationProvider, QuestionMenu>();
            services.AddScoped<INavigationProvider, QuestionFooterMenu>();
            services.AddScoped<INavigationProvider, AnswerMenu>();
            services.AddScoped<INavigationProvider, AnswerFooterMenu>();
         
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