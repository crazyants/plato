﻿using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plato.Abstractions.SetUp;
using Plato.Hosting;
using Plato.Hosting.Extensions;
using Plato.Models.Roles;
using Plato.Models.Users;
using Plato.Shell.Models;
using Plato.Stores.Roles;
using Plato.Stores.Users;
using Plato.Hosting.Web;
using Plato.Users.Services;

namespace Plato.Users
{
    public class Startup : StartupBase
    {
        //private readonly IdentityOptions _options;

        private readonly string _tenantName;
        private readonly string _cookieSuffix;
        private readonly string _tenantPrefix;
      

        public Startup(
            ShellSettings shellSettings)
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
            services.TryAddScoped<IRoleStore<Role>, RoleStore>();
            services.TryAddScoped<UserManager<User>>();
            services.TryAddScoped<RoleManager<Role>>();
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


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // add authentication middleware
            app.UseAuthentication();

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