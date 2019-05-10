using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Private.Navigation;
using Plato.Questions.Models;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Security.Abstractions;
using Plato.Questions.Private.Handlers;
using Plato.Questions.Private.ViewProviders;

namespace Plato.Questions.Private
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

            // View providers
            services.AddScoped<IViewProviderManager<Question>, ViewProviderManager<Question>>();
            services.AddScoped<IViewProvider<Question>, QuestionViewProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, PostMenu>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}