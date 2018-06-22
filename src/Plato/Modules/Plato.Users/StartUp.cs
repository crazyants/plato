using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Hosting;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Hosting.Web;
using Plato.Internal.Hosting.Web.Extensions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.TagHelpers;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Users.ViewAdaptors;
using Plato.Users.Services;
using Plato.Users.ViewModels;
using Plato.Users.ViewProviders;

namespace Plato.Users
{
    public class Startup : StartupBase
    {
        //private readonly IdentityOptions _options;

        private readonly string _tenantName;
        private readonly string _cookieSuffix;
        private readonly string _tenantPrefix;

        public Startup(
            IShellSettings shellSettings)
        {
            //_options = options;
            _tenantName = shellSettings.Name;
            _cookieSuffix = shellSettings.AuthCookieName;
            _tenantPrefix = shellSettings.RequestedUrlPrefix;
      
        }

        public override void ConfigureServices(IServiceCollection services)
        {
          
            // register set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Adds the default token providers used to generate tokens for reset passwords, change email
            // and change telephone number operations, and for two factor authentication token generation.
            new IdentityBuilder(typeof(User), typeof(Role), services).AddDefaultTokenProviders();
        
            // --------

            services.TryAddScoped<IUserValidator<User>, UserValidator<User>>();
            services.TryAddScoped<IPasswordValidator<User>, PasswordValidator<User>>();
            services.TryAddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<User>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, Role>>();

            services.TryAddScoped<IUserStore<User>, UserStore>();
     
            services.TryAddScoped<UserManager<User>>();
            services.TryAddScoped<SignInManager<User>>();

            services.AddSingleton<IContextFacade, ContextFacade>();
            
            // configurate authentication cookie

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "plato_" + _cookieSuffix.ToLower();
                options.Cookie.Path = _tenantPrefix;
                options.LoginPath = new PathString("/Plato.Users/Account/Login/");
                options.AccessDeniedPath = new PathString("/Plato.Users/Account/Login/");
                options.AccessDeniedPath = options.LoginPath;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

            // register navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // register view proviers

            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            services.AddScoped<IViewProviderManager<UsersIndexViewModel>, ViewProviderManager<UsersIndexViewModel>>();
            services.AddScoped<IViewProvider<UsersIndexViewModel>, UserIndexViewProvider>();

            //services.AddScoped<IViewProvider<User>, UserViewProvider>();


            // register view drivers
            services.AddScoped<IViewAdaptorProvider, UserListAdaptor>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // load tag helpers
            serviceProvider.AddTagHelpers(typeof(PagerTagHelper).Assembly);


            // add authentication middleware
            app.UseAuthentication();

            routes.MapAreaRoute(
                name: "Login",
                areaName: "Plato.Users",
                template: "login",
                defaults: new { controller = "Account", action = "Login" }
            );

            routes.MapAreaRoute(
                name: "Register",
                areaName: "Plato.Users",
                template: "register",
                defaults: new { controller = "Account", action = "Register" }
            );

            routes.MapAreaRoute(
                name: "Admin-ManageUsers",
                areaName: "Plato.Users",
                template: "admin/users",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "Admin-DisplayUser",
                areaName: "Plato.Users",
                template: "admin/users/display",
                defaults: new { controller = "Admin", action = "Display" }
            );


            routes.MapAreaRoute(
                name: "Admin-CreateUser",
                areaName: "Plato.Users",
                template: "admin/users/create",
                defaults: new { controller = "Admin", action = "Create" }
            );
            
            routes.MapAreaRoute(
                name: "Admin-EditUser",
                areaName: "Plato.Users",
                template: "admin/users/edit",
                defaults: new { controller = "Admin", action = "EditAsync" }
            );


            //routes.MapAreaRoute(
            //    viewName: "Users",
            //    area: "Plato.Users",
            //    template: "users/{controller}/{action}/{id?}",
            //    controller: "Account",
            //    action: "Login"
            //);


        }
    }
}