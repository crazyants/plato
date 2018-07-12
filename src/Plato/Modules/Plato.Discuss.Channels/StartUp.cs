using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Channels.ViewProviders;
using Plato.Discuss.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels
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

            //// Feature installation event handler
            //services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, ChannelsViewProvider>();
            
            //// Repositories
            //services.AddScoped<IEmailRepository<EmailMessage>, EmailRepository>();

            //// Stores
            //services.AddScoped<IEmailSettingsStore<EmailSettings>, EmailSettingsStore>();

            //// Email manager
            //services.AddSingleton<IEmailManager, EmailManager>();

            //// Smtp settings & service
            //services.AddTransient<IConfigureOptions<SmtpSettings>, SmtpSettingsConfiguration>();
            //services.AddScoped<ISmtpService, SmtpService>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {




        }
    }
}