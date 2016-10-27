using System;
using Plato.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Plato.Hosting.Extensions;
using Plato.Shell.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity;
using Plato.Models.Users;
using Plato.Models.Roles;
using Plato.Repositories.Users;
using Plato.Stores.Users;

namespace Plato.Login
{

    public class Startup : StartupBase
    {

        private readonly string _tenantName;
        private readonly string _tenantPrefix;
        private readonly IdentityOptions _options;
        

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
            serviceCollection.TryAddScoped<SignInManager<User>>();

            serviceCollection.TryAddScoped<IUserRepository<User>, UserRepository>();
            serviceCollection.TryAddScoped<IUserStore<User>, UserStore>();


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
                name: "Login",
                area: "Plato.Login",
                template: "admin",
                controller: "Login",
                action: "Index"
            );
        }

    }



}
