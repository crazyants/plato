using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Layout.ViewAdaptors;
using Plato.Layout.ModelBinding;
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
            services.AddSingleton<IGenericViewFactory, ViewFactory>();
            services.AddSingleton<IGenericViewTableManager, ViewTableManager>();
            services.AddSingleton<IGenericViewInvoker, ViewInvoker>();
            
            // add theming convension - configures theme layout based on controller type
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            // model binding filters
            services.Configure<MvcOptions>((options) =>
            {
                options.Filters.Add(typeof(ModelBinderAccessorFilter));
            });

            // model binding model accessor
            services.AddScoped<IUpdateModelAccessor, LocalModelBinderAccessor>();

            return services;

        }
    }
}
