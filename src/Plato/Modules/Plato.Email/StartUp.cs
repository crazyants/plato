using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Email.Configuration;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Email.Handlers;
using Plato.Email.Models;
using Plato.Email.Navigation;
using Plato.Email.Repositories;
using Plato.Email.Services;
using Plato.Email.Stores;
using Plato.Email.Subscribers;
using Plato.Email.ViewProviders;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Email
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Repositories
            services.AddScoped<IEmailRepository<EmailMessage>, EmailRepository>();

            // Stores
            services.AddScoped<IEmailSettingsStore<EmailSettings>, EmailSettingsStore>();
            services.AddScoped<IEmailStore<EmailMessage>, EmailStore>();

            // View providers
            services.AddScoped<IViewProviderManager<EmailSettings>, ViewProviderManager<EmailSettings>>();
            services.AddScoped<IViewProvider<EmailSettings>, AdminViewProvider>();

            // Configuration
            services.AddTransient<IConfigureOptions<SmtpSettings>, SmtpSettingsConfiguration>();
       
            // Services
            services.AddScoped<ISmtpService, SmtpService>();

            // Email subscribers
            services.AddScoped<IBrokerSubscriber, EmailSubscriber>();

            // Email manager
            services.AddSingleton<IEmailManager, EmailManager>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoEmailAdmin",
                areaName: "Plato.Email",
                template: "admin/settings/email",
                defaults: new { controller = "Admin", action = "Index" }
            );


        }

    }

}