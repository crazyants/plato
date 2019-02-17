using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.Notifications;
using Plato.Users.StopForumSpam.NotificationTypes;
using Plato.Users.StopForumSpam.ViewProviders;
using Plato.Users.StopForumSpam.SpamOperators;


namespace Plato.Users.StopForumSpam
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
            
            // Register spam operations 
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();
            
            // Register spam operator manager for users
            services.AddScoped<ISpamOperatorManager<User>, SpamOperatorManager<User>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<User>, RegistrationOperator>();
            services.AddScoped<ISpamOperatorProvider<User>, LoginOperator>();

            // Login view provider
            services.AddScoped<IViewProviderManager<UserLogin>, ViewProviderManager<UserLogin>>();
            services.AddScoped<IViewProvider<UserLogin>, LoginViewProvider>();

            // Registration view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();

            // Admin view provider
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, AdminViewProvider>();

            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<User>, NotificationManager<User>>();

            // Notification providers
            services.AddScoped<INotificationProvider<User>, NewSpamWeb>();
            

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}