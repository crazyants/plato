using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Channels.Follow.Subscribers;
using Plato.Discuss.Channels.Follow.ViewProviders;
using Plato.Discuss.Channels.Models;
using Plato.Follows.Services;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Follow
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

            // Channel View providers
            services.AddScoped<IViewProviderManager<Channel>, ViewProviderManager<Channel>>();
            services.AddScoped<IViewProvider<Channel>, ChannelViewProvider>();

            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}