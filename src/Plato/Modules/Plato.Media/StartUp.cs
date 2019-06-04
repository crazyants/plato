using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Media.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Media.Repositories;
using Plato.Media.Stores;

namespace Plato.Media
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

            // Data 
            services.AddScoped<IMediaRepository<Models.Media>, MediaRepository>();
            services.AddScoped<IMediaStore<Models.Media>, MediaStore>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // api routes
            routes.MapAreaRoute(
                name: "MediaWebApi",
                areaName: "Plato.Media",
                template: "api/media/{controller}/{action}/{id?}",
                defaults: new { controller = "Upload", action = "Index" }
            );
            
            // serve media routes
            routes.MapAreaRoute(
                name: "ServeMedia",
                areaName: "Plato.Media",
                template: "media/{id?}",
                defaults: new { controller = "Media", action = "Serve" }
            );


        }
    }
}