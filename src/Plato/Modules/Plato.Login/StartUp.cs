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
using Plato.Repositories.Users;
using Plato.Shell.Models;
using Plato.Stores.Users;
using Plato.Stores.Roles;

namespace Plato.Login
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

        public override void ConfigureServices(IServiceCollection serviceCollection)
        {
            new IdentityBuilder(typeof(User), typeof(Role), serviceCollection).AddDefaultTokenProviders();

            // Identity services
            serviceCollection.TryAddSingleton<IdentityMarkerService>();
            serviceCollection.TryAddScoped<IUserValidator<User>, UserValidator<User>>();
            serviceCollection.TryAddScoped<IPasswordValidator<User>, PasswordValidator<User>>();
            serviceCollection.TryAddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            serviceCollection.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            // No interface for the error describer so we can add errors without rev'ing the interface
            serviceCollection.TryAddScoped<IdentityErrorDescriber>();
            serviceCollection.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<User>>();
            serviceCollection.TryAddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, Role>>();
            serviceCollection.TryAddScoped<UserManager<User>>();
            serviceCollection.TryAddScoped<RoleManager<Role>>();
            serviceCollection.TryAddScoped<SignInManager<User>>();
            //serviceCollection.TryAddScoped<IUserRepository<User>, UserRepository>();
            serviceCollection.TryAddScoped<IUserStore<User>, UserStore>();

           

            serviceCollection.Configure<IdentityOptions>(options =>
            {
                options.Cookies.ApplicationCookie.CookieName = "platoauth_" + _tenantName;
                options.Cookies.ApplicationCookie.CookiePath = _tenantPrefix;
                options.Cookies.ApplicationCookie.LoginPath = new PathString("/Plato.Login/");
                options.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/Plato.Login/");
            });


        }

        public override void Configure(
            IApplicationBuilder builder,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            builder.UseIdentity();
            builder
                .UseCookieAuthentication(_options.Cookies.ApplicationCookie)
                .UseCookieAuthentication(_options.Cookies.ExternalCookie)
                .UseCookieAuthentication(_options.Cookies.TwoFactorRememberMeCookie)
                .UseCookieAuthentication(_options.Cookies.TwoFactorUserIdCookie);

            routes.MapAreaRoute(
                "Login",
                "Plato.Login",
                "admin",
                "Login",
                "Index"
            );
        }
    }
}