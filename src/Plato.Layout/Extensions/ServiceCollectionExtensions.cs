using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Plato.Layout.Theming;

namespace Plato.Layout.Extensions
{
    public static class ServiceCollectionExtensions
    {
    
        public static IServiceCollection AddPlatoViewFeature(
            this IServiceCollection services)
        {
            
            // add theming features - configures theme layout based on controller type
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            return services;

        }
    }
}
