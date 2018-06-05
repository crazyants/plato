using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Layout.TagHelpers;
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
        
        public static IServiceCollection AddPlatoViewFeature(
            this IServiceCollection services)
        {

            services.AddSingleton<IViewHelperFactory, ViewDisplayHelperFactory>();
            services.AddSingleton<IGenericViewFactory, GenericViewFactory>();
            services.AddSingleton<IHtmlDisplay, DefaultHtmlDisplay>();

            services.AddSingleton<IGenericViewTableManager, GenericViewTableManager>();
            services.AddSingleton<IGenericViewInvoker, GenericViewInvoker>();

            



            // add theming convension - configures theme layout based on controller type
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            return services;

        }
    }
}
