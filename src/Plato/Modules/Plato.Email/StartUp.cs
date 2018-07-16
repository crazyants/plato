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
using Plato.Email.Repositories;
using Plato.Email.Services;
using Plato.Email.Stores;

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

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Repositories
            services.AddScoped<IEmailRepository<EmailMessage>, EmailRepository>();

            // Stores
            services.AddScoped<IEmailSettingsStore<EmailSettings>, EmailSettingsStore>();

            // Email manager
            services.AddSingleton<IEmailManager, EmailManager>();

            // Smtp settings & service
            services.AddTransient<IConfigureOptions<SmtpSettings>, SmtpSettingsConfiguration>();
            services.AddScoped<ISmtpService, SmtpService>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

       

        }
    }
}