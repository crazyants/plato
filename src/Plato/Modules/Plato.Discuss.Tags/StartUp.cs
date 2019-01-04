using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Badges;
using Plato.Discuss.Tags.Navigation;
using Plato.Discuss.Tags.Services;
using Plato.Discuss.Tags.Tasks;
using Plato.Discuss.Tags.ViewProviders;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Navigation;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Tags.Models;

namespace Plato.Discuss.Tags
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

            // Tag service
            services.AddScoped<ITagService, TagService>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, TopicFooterMenu>();
            services.AddScoped<INavigationProvider, TopicReplyFooterMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();

            // Tag view providers
            services.AddScoped<IViewProviderManager<Tag>, ViewProviderManager<Tag>>();
            services.AddScoped<IViewProvider<Tag>, TagViewProvider>();
            
            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, TagBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, TagBadgesAwarder>();

            // Notification manager
            services.AddScoped<INotificationManager<Badge>, NotificationManager<Badge>>();

          

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DiscussTagIndex",
                areaName: "Plato.Discuss.Tags",
                template: "discuss/tags/{offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DiscussTagDisplay",
                areaName: "Plato.Discuss.Tags",
                template: "discuss/tag/{id:int}/{alias?}",
                defaults: new { controller = "Home", action = "Display" }
            );


        }
    }
}