using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Layout.Adaptors;
using Plato.Layout.Theming;
using Plato.Layout.Views;

namespace Plato.Layout.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatoTagHelpers(
            this IServiceCollection services)
        {
         
            return services;

        }

        public static IServiceCollection AddPlatoViewAdaptors(
            this IServiceCollection services)
        {
            // view adaptors
            services.TryAddScoped<IViewAdaptorManager, ViewAdaptorManager>();

            return services;

        }


        public static IServiceCollection AddPlatoViewFeature(
            this IServiceCollection services)
        {
          
            // gneric views
            services.AddSingleton<IViewHelperFactory, ViewDisplayHelperFactory>();
            services.AddSingleton<IGenericViewFactory, GenericViewFactory>();
            services.AddSingleton<IGenericViewTableManager, GenericViewTableManager>();
            services.AddSingleton<IGenericViewInvoker, GenericViewInvoker>();
            
            // add theming convension - configures theme layout based on controller type
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            return services;

        }
    }
}
