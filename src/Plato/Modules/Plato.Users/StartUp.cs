using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plato.Hosting;
using Plato.Hosting.Extensions;
using Plato.Models.Roles;
using Plato.Models.Users;
using Plato.Shell.Models;
using Plato.Stores.Roles;
using Plato.Stores.Users;
using Plato.Hosting.Web;

namespace Plato.Users
{
    public class Startup : StartupBase
    {
        private readonly IdentityOptions _options;

        private readonly string _tenantName;
        private readonly string _tenantPrefix;
        
        public Startup(
            ShellSettings shellSettings,
            IOptions<IdentityOptions> options)
        {
            _options = options.Value;
            _tenantName = shellSettings.Name;
            _tenantPrefix = shellSettings.RequestedUrlPrefix;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            new IdentityBuilder(
                typeof(User), 
                typeof(Role), 
                services).AddDefaultTokenProviders();

            // Identity services
            //services.TryAddSingleton<IdentityMarkerService>();
            services.TryAddScoped<IUserValidator<User>, UserValidator<User>>();
            services.TryAddScoped<IPasswordValidator<User>, PasswordValidator<User>>();
            services.TryAddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<User>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, Role>>();

            services.TryAddScoped<IUserStore<User>, UserStore>();
            services.TryAddScoped<IRoleStore<Role>, RoleStore>();
            services.TryAddScoped<UserManager<User>>();
            services.TryAddScoped<RoleManager<Role>>();
            services.TryAddScoped<SignInManager<User>>();

            services.AddSingleton<IContextFacade, ContextFacade>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Plato.Users/Account/Login/");
                options.AccessDeniedPath = new PathString("/Plato.Users/Account/Login/");
                options.Cookie.Name = "platoauth_" + _tenantName;
                options.Cookie.Path = _tenantPrefix;
            });

            // .net core 1.x implementation
            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Cookies.ApplicationCookie.CookieName = "platoauth_" + _tenantName;
            //    options.Cookies.ApplicationCookie.CookiePath = _tenantPrefix;
            //    options.Cookies.ApplicationCookie.LoginPath = new PathString("/Plato.Users/Account/Login/");
            //    options.Cookies.ApplicationCookie.AutomaticChallenge = true;
            //    options.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/Plato.Users/Account/Login/");
            //});

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            app.UseAuthentication();

            //app.UseIdentity();
            //app.UseCookieAuthentication(_options.Cookies.ApplicationCookie)
            //    .UseCookieAuthentication(_options.Cookies.ExternalCookie)
            //    .UseCookieAuthentication(_options.Cookies.TwoFactorRememberMeCookie)
            //    .UseCookieAuthentication(_options.Cookies.TwoFactorUserIdCookie);

            routes.MapAreaRoute(
                name: "Login",
                area: "Plato.Users",
                template: "login",
                controller: "Account",
                action: "Login"
            );

            routes.MapAreaRoute(
                name: "Register",
                area: "Plato.Users",
                template: "register",
                controller: "Account",
                action: "Register"
            );
            
            routes.MapAreaRoute(
                name: "Users",
                area: "Plato.Users",
                template: "users/{controller}/{action}/{id?}",
                controller: "Account",
                action: "Login"
            );


        }
    }
}