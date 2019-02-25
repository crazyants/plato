using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Services;
using Plato.Users.reCAPTCHA2.Navigation;
using Plato.Users.reCAPTCHA2.Stores;
using Plato.Users.reCAPTCHA2.ViewProviders;

namespace Plato.Users.reCAPTCHA2
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
            // Stores
            services.AddScoped<IReCaptchaSettingsStore<ReCaptchaSettings>, ReCaptchaSettingsStore>();

            // Login view provider
            services.AddScoped<IViewProviderManager<UserLogin>, ViewProviderManager<UserLogin>>();
            services.AddScoped<IViewProvider<UserLogin>, LoginViewProvider>();

            // Register view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();
            
            // Admin view provider
            services.AddScoped<IViewProviderManager<ReCaptchaSettings>, ViewProviderManager<ReCaptchaSettings>>();
            services.AddScoped<IViewProvider<ReCaptchaSettings>, AdminViewProvider>();
            
            // Register services
            services.AddScoped<IReCaptchaService, ReCaptchaService>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}