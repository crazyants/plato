using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Categories.Models;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Roles;
using Plato.Internal.Hosting.Abstractions;
using Plato.Roles.ViewProviders;
using Plato.Roles.Handlers;
using Plato.Roles.Navigation;
using Plato.Roles.Services;


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
            
            // Replace dummy role stores registered via User StartUp with real implementations
            services.TryAddScoped<IRoleStore<Role>, RoleStore>();
            services.TryAddScoped<IRoleClaimStore<Role>, RoleStore>();
       
            // Register role manager
            services.TryAddScoped<RoleManager<Role>>();

            // User view providers
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            // Category view providers
            services.AddScoped<IViewProviderManager<CategoryBase>, ViewProviderManager<CategoryBase>>();
            services.AddScoped<IViewProvider<CategoryBase>, CategoryViewProvider>();
            
            // Role view provider
            services.AddScoped<IViewProviderManager<Role>, ViewProviderManager<Role>>();
            services.AddScoped<IViewProvider<Role>, AdminViewProvider>();
            
            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Register moderation permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

      

            // Register feature & set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();
            //services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Register additional authorization handler for implied permissions
            services.AddScoped<IAuthorizationHandler, RolesPermissionsHandler>();

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