using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Discuss.Share.Navigation;

namespace Plato.Discuss.Share
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

            // Navigation providers
            services.AddScoped<INavigationProvider, TopicMenu>();
            services.AddScoped<INavigationProvider, TopicReplyMenu>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DiscussTopicShare",
                areaName: "Plato.Discuss.Share",
                template: "discuss/topic/share/{entityId}/{alias}/{entityReplyId?}",
                defaults: new { controller = "Home", action = "Index", entityReplyId  = "0"}
            );


        }
    }
}