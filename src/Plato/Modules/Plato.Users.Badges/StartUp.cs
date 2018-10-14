using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Internal.Hosting;
using Plato.Internal.Abstractions.SetUp;
using Plato.Demo.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Users.Badges.Providers;
using Plato.Users.Badges.ViewProviders;

namespace Plato.Users.Badges
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

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, VisitBadges>();

            // User profile view proviers
            services.AddScoped<IViewProviderManager<UserProfile>, ViewProviderManager<UserProfile>>();
            services.AddScoped<IViewProvider<UserProfile>, UserViewProvider>();

            // Badge view proviers
            services.AddScoped<IViewProviderManager<UserBadge>, ViewProviderManager<UserBadge>>();
            services.AddScoped<IViewProvider<UserBadge>, UserBadgeViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}