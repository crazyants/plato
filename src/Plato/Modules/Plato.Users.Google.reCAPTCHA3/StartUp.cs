using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.Google.reCAPTCHA3.Services;
using Plato.Users.Google.reCAPTCHA3.ViewProviders;
using Plato.Users.ViewModels;

namespace Plato.Users.Google.reCAPTCHA3
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

            // Register services
            services.AddScoped<IReCaptchaService, ReCaptchaService>();
            
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}