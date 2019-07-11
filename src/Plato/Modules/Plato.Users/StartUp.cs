using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Security;
using Plato.Users.ViewAdaptors;
using Plato.Users.Handlers;
using Plato.Users.ViewModels;
using Plato.Users.ViewProviders;
using Plato.Internal.Security.Abstractions;
using Plato.Users.ActionFilters;
using Plato.Users.Middleware;
using Plato.Users.Navigation;
using Plato.Users.Services;
using Plato.Users.Subscribers;

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
          
            // Register set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Register feature event handlers
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
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

            // Custom UserClaimsPrincipalFactory implementations
            services.TryAddScoped<IUserClaimsPrincipalFactory<User>, PlatoClaimsPrincipalFactory<User, Role>>();
            services.TryAddScoped<IDummyClaimsPrincipalFactory<User>, DummyClaimsPrincipalFactory<User>>();
            
            // Stores
            services.TryAddScoped<IUserStore<User>, UserStore>();
            services.TryAddScoped<IUserSecurityStampStore<User>, UserStore>();

            // Managers
            services.TryAddScoped<UserManager<User>>();
            services.TryAddScoped<SignInManager<User>>();

            // User color provider 
            services.AddSingleton<IUserColorProvider, UserColorProvider>();

            // Custom User Manager
            services.AddScoped<IPlatoUserManager<User>, PlatoUserManager<User>>();

            // User account emails
            services.TryAddScoped<IUserEmails, UserEmails>();
            
            // Configure authentication cookie options
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
            services.AddScoped<INavigationProvider, AdminUserMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, EditProfileMenu>();
            services.AddScoped<INavigationProvider, ProfileMenu>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, AdminViewProvider>();
          
            // Profile view providers
            services.AddScoped<IViewProviderManager<Profile>, ViewProviderManager<Profile>>();
            services.AddScoped<IViewProvider<Profile>, UserViewProvider>();

            // Edit profile view provider
            services.AddScoped<IViewProviderManager<EditProfileViewModel>, ViewProviderManager<EditProfileViewModel>>();
            services.AddScoped<IViewProvider<EditProfileViewModel>, EditProfileViewProvider>();

            // Edit account view provider
            services.AddScoped<IViewProviderManager<EditAccountViewModel>, ViewProviderManager<EditAccountViewModel>>();
            services.AddScoped<IViewProvider<EditAccountViewModel>, EditAccountViewProvider>();
            
            // Edit user settings view provider
            services.AddScoped<IViewProviderManager<EditSettingsViewModel>, ViewProviderManager<EditSettingsViewModel>>();
            services.AddScoped<IViewProvider<EditSettingsViewModel>, EditSettingsViewProvider>();
           
            // Edit user signature view provider
            services.AddScoped<IViewProviderManager<EditSignatureViewModel>, ViewProviderManager<EditSignatureViewModel>>();
            services.AddScoped<IViewProvider<EditSignatureViewModel>, EditSignatureViewProvider>();

            // Login view provider
            services.AddScoped<IViewProviderManager<UserLogin>, ViewProviderManager<UserLogin>>();
            services.AddScoped<IViewProvider<UserLogin>, LoginViewProvider>();

            // Register view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // register view adapters
            services.AddScoped<IViewAdapterProvider, UserListAdapter>();
            
            // Register reputation providers
            services.AddScoped<IReputationsProvider<Reputation>, Reputations>();

            // Register user service
            services.AddScoped<IUserService<User>, UserService<User>>();

            // Register action filters
            services.AddScoped<IModularActionFilter, SignOutIfUserNotFoundFilter>();
            services.AddScoped<IModularActionFilter, UpdateUserLastLoginDateFilter>();

            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

            // Broker subscriptions
            services.AddScoped<IBrokerSubscriber, ParseSignatureHtmlSubscriber>();

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
            
            // --------------
            // Account
            // --------------

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

            // --------------
            // Users
            // --------------

            routes.MapAreaRoute(
                name: "UsersIndex",
                areaName: "Plato.Users",
                template: "u/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DisplayUser",
                areaName: "Plato.Users",
                template: "u/{opts.id:int}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            routes.MapAreaRoute(
                name: "GetUser",
                areaName: "Plato.Users",
                template: "u/get/{opts.id:int}/{opts.alias?}",
                defaults: new { controller = "Home", action = "GetUser" }
            );


            routes.MapAreaRoute(
                name: "UserLetter",
                areaName: "Plato.Users",
                template: "u/l/{letter}/{color}",
                defaults: new { controller = "Letter", action = "Get" }
            );
            
            // --------------
            // Profile
            // --------------

            routes.MapAreaRoute(
                name: "EditUserProfile",
                areaName: "Plato.Users",
                template: "profile",
                defaults: new { controller = "Home", action = "EditProfile" }
            );
            
            routes.MapAreaRoute(
                name: "EditUserAccount",
                areaName: "Plato.Users",
                template: "profile/account",
                defaults: new { controller = "Home", action = "EditAccount" }
            );

            routes.MapAreaRoute(
                name: "EditUserSignature",
                areaName: "Plato.Users",
                template: "profile/signature",
                defaults: new { controller = "Home", action = "EditSignature" }
            );

            routes.MapAreaRoute(
                name: "EditUserSettings",
                areaName: "Plato.Users",
                template: "profile/settings",
                defaults: new { controller = "Home", action = "EditSettings" }
            );
            
            // --------------
            // Admin Routes
            // --------------

            routes.MapAreaRoute(
                name: "AdminUsersOffset",
                areaName: "Plato.Users",
                template: "admin/users/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "Admin-Users",
                areaName: "Plato.Users",
                template: "admin/users/{action}/{id?}",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }
    
    }

}