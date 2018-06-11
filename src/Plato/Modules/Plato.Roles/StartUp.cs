using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Hosting;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Models;
using Plato.Roles.Services;

namespace Plato.Roles
{
    public class Startup : StartupBase
    {
        private readonly ShellSettings _shellSettings;

        public Startup(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // register set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // register role stores
            services.TryAddScoped<IRoleStore<Role>, RoleStore>();
            services.TryAddScoped<IRoleClaimStore<Role>, RoleStore>();

            // register role manager
            services.TryAddScoped<RoleManager<Role>>();

            // register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "ManageRoles",
                areaName: "Plato.Roles",
                template: "admin/roles",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }
    }
}