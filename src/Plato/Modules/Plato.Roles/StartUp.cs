using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Hosting;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;
using Plato.Roles.Services;
using Plato.Roles.ViewModels;
using Plato.Roles.ViewProviders;

namespace Plato.Roles
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

            // register set-up event handler

            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // register role stores

            services.TryAddScoped<IRoleStore<Role>, RoleStore>();
            services.TryAddScoped<IRoleClaimStore<Role>, RoleStore>();

            // register role manager

            services.TryAddScoped<RoleManager<Role>>();

            // user view providers

            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            // role view providers

            services.AddScoped<IViewProviderManager<RolesIndexViewModel>, ViewProviderManager<RolesIndexViewModel>>();
            services.AddScoped<IViewProvider<RolesIndexViewModel>, RoleIndexViewProvider>();

            services.AddScoped<IViewProviderManager<Role>, ViewProviderManager<Role>>();
            services.AddScoped<IViewProvider<Role>, RoleViewProvider>();
            
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