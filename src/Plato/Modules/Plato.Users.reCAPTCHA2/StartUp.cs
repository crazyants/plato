using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Users.ViewModels;
using Plato.Users.reCAPTCHA2.Models;
using Plato.Users.reCAPTCHA2.Services;
using Plato.Users.reCAPTCHA2.Navigation;
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

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginViewModel>, ViewProviderManager<LoginViewModel>>();
            services.AddScoped<IViewProvider<LoginViewModel>, LoginViewProvider>();

            // Register view provider
            services.AddScoped<IViewProviderManager<RegisterViewModel>, ViewProviderManager<RegisterViewModel>>();
            services.AddScoped<IViewProvider<RegisterViewModel>, RegisterViewProvider>();
            
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