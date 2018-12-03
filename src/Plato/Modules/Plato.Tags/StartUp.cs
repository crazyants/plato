using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Tags.Handlers;
using Plato.Tags.Models;
using Plato.Tags.Repositories;
using Plato.Tags.Stores;

namespace Plato.Tags
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

            // Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
            // Repositories
            services.AddScoped<ITagRepository<Tag>, TagRepository>();
            services.AddScoped<IEntityTagsRepository<EntityTag>, EntityTagsRepository>();

            // Stores
            services.AddScoped<ITagStore<Tag>, TagStore>();
            services.AddScoped<IEntityTagStore<EntityTag>, EntityTagStore>();



        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}