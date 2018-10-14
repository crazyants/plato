using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Handlers;
using Plato.Badges.Models;
using Plato.Badges.Providers;
using Plato.Badges.Repositories;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Badges.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;

namespace Plato.Badges
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
            services.AddScoped<IUserBadgeRepository<UserBadge>, UserBadgeRepository>();

            // Stores
            services.AddScoped<IUserBadgeStore<UserBadge>, UserBadgeStore>();
            
            // Services
            services.AddScoped<IBadgesManager<Badge>, BadgesManager<Badge>>();
            services.AddScoped<IBadgeAwarderInvoker, BadgeAwarderInvoker>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, VisitBadges>();
            
            // Profile view proviers
            services.AddScoped<IViewProviderManager<UserProfile>, ViewProviderManager<UserProfile>>();
            services.AddScoped<IViewProvider<UserProfile>, UserViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Activate all registered badge awarders
            var awarders = serviceProvider.GetServices<IBadgeAwarderInvoker>();
            foreach (var awarder in awarders)
            {
                awarder?.Invoke();
            }


        }
    }
}