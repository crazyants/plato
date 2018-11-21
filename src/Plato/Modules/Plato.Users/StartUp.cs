using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Users.ViewAdaptors;
using Plato.Users.Handlers;
using Plato.Users.ViewModels;
using Plato.Users.ViewProviders;
using Plato.Internal.Security.Abstractions;
using Plato.Users.Middleware;
using Plato.Users.Navigation;
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
            
            // Adds default token providers
            new IdentityBuilder(typeof(User), typeof(Role), services).AddDefaultTokenProviders();
        
            // --------

            // .NET core implementations
            services.TryAddScoped<IUserValidator<User>, UserValidator<User>>();
            services.TryAddScoped<IPasswordValidator<User>, PasswordValidator<User>>();
            services.TryAddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<User>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, Role>>();

            // Stores
            services.TryAddScoped<IUserStore<User>, UserStore>();
     
            // Managers
            services.TryAddScoped<UserManager<User>>();
            services.TryAddScoped<SignInManager<User>>();

            // Custom User Manager
            services.AddScoped<IPlatoUserManager<User>, PlatoUserManager<User>>();

            // Context facade
            //services.TryAddScoped<IContextFacade, ContextFacade>();
            
            // Configurate authentication cookie options
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

            // Configure IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            // Navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, EditProfileMenu>();
            services.AddScoped<INavigationProvider, ProfileMenu>();

            // Admin view proviers
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, AdminViewProvider>();
          
            // Profile view proviers
            services.AddScoped<IViewProviderManager<UserProfile>, ViewProviderManager<UserProfile>>();
            services.AddScoped<IViewProvider<UserProfile>, UserViewProvider>();

            // Edit profile view provider
            services.AddScoped<IViewProviderManager<EditProfileViewModel>, ViewProviderManager<EditProfileViewModel>>();
            services.AddScoped<IViewProvider<EditProfileViewModel>, EditProfileViewProvider>();

            // Edit account view provider
            services.AddScoped<IViewProviderManager<EditAccountViewModel>, ViewProviderManager<EditAccountViewModel>>();
            services.AddScoped<IViewProvider<EditAccountViewModel>, EditAccountViewProvider>();
            
            // Edit user settings view provider
            services.AddScoped<IViewProviderManager<EditSettingsViewModel>, ViewProviderManager<EditSettingsViewModel>>();
            services.AddScoped<IViewProvider<EditSettingsViewModel>, EditSettingsViewProvider>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // register view drivers
            services.AddScoped<IViewAdaptorProvider, UserListAdaptor>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // add authentication middleware
            app.UseAuthentication();

            // Register authenticated user middleware
            // Must be registered after .NET authentication middleware has been registered
            // i.e. after app.UseAuthentication() above
            app.UseMiddleware<AuthenticatedUserMiddleware>();
          
            // Routes

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
                name: "ConfirmEmail",
                areaName: "Plato.Users",
                template: "account/confirm",
                defaults: new { controller = "Account", action = "ConfirmEmail" }
            );

            routes.MapAreaRoute(
                name: "ConfirmEmailConfirmation",
                areaName: "Plato.Users",
                template: "account/confirm/success",
                defaults: new { controller = "Account", action = "ConfirmEmailConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ActivateAccount",
                areaName: "Plato.Users",
                template: "account/activate",
                defaults: new { controller = "Account", action = "ActivateAccount" }
            );

            routes.MapAreaRoute(
                name: "ActivateAccountConfirmation",
                areaName: "Plato.Users",
                template: "account/activate/success",
                defaults: new { controller = "Account", action = "ActivateAccountConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ForgotPassword",
                areaName: "Plato.Users",
                template: "account/forgot",
                defaults: new { controller = "Account", action = "ForgotPassword" }
            );
            
            routes.MapAreaRoute(
                name: "ForgotPasswordConfirmation",
                areaName: "Plato.Users",
                template: "account/forgot/success",
                defaults: new { controller = "Account", action = "ForgotPasswordConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ResetPassword",
                areaName: "Plato.Users",
                template: "account/reset",
                defaults: new { controller = "Account", action = "ResetPassword" }
            );

            routes.MapAreaRoute(
                name: "ResetPasswordConfirmation",
                areaName: "Plato.Users",
                template: "account/reset/success",
                defaults: new { controller = "Account", action = "ResetPasswordConfirmation" }
            );


            routes.MapAreaRoute(
                name: "Admin-Users",
                areaName: "Plato.Users",
                template: "admin/users/{action}/{id?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "Home-Users",
                areaName: "Plato.Users",
                template: "users",
                defaults: new { controller = "Home", action = "Index"}
            );

            routes.MapAreaRoute(
                name: "ServeUserPhotoFolder",
                areaName: "Plato.Users",
                template: "users/photo",
                defaults: new { controller = "Photo", action = "Serve", id = "0"}
            );

            routes.MapAreaRoute(
                name: "ServeUserPhoto",
                areaName: "Plato.Users",
                template: "users/photo/{id}",
                defaults: new { controller = "Photo", action = "Serve" }
            );

            routes.MapAreaRoute(
                name: "DisplayUserProfile",
                areaName: "Plato.Users",
                template: "users/{id}/{alias?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            routes.MapAreaRoute(
                name: "EditUserProfile",
                areaName: "Plato.Users",
                template: "profile/edit",
                defaults: new { controller = "Home", action = "EditProfile" }
            );
            
            routes.MapAreaRoute(
                name: "EditUserAccount",
                areaName: "Plato.Users",
                template: "account/edit",
                defaults: new { controller = "Home", action = "EditAccount" }
            );

            routes.MapAreaRoute(
                name: "EditUserSettings",
                areaName: "Plato.Users",
                template: "settings/edit",
                defaults: new { controller = "Home", action = "EditSettings" }
            );



        }

    }

}