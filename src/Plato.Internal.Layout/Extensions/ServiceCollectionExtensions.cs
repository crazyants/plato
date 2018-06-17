using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.Theming;
using Plato.Internal.Layout.Views;
using Plato.Internal.Navigation.Extensions;

namespace Plato.Internal.Layout.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatoTagHelpers(
            this IServiceCollection services)
        {
            services.AddPlatoNavigation();
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

            // layout updater
            services.AddSingleton<ILayoutUpdater, LayoutUpdater>();
            
            // gneric views
            services.AddSingleton<IViewHelperFactory, ViewDisplayHelperFactory>();
            services.AddSingleton<IViewFactory, ViewFactory>();
            services.AddSingleton<IViewTableManager, ViewTableManager>();
            services.AddSingleton<IViewInvoker, ViewInvoker>();

            // add theming convension - configures theme layout based on controller type
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            // action filters
            services.Configure<MvcOptions>((options) =>
            {
                options.Filters.Add(typeof(ModelBinderAccessorFilter));
                options.Filters.Add(typeof(AlertFilter));
            });

            // model binding model accessor
            services.AddScoped<IUpdateModelAccessor, LocalModelBinderAccessor>();

            // alerter - scoped to ensure alerter is only alive for each request
            services.AddScoped<IAlerter, Alerter>();
            
            return services;

        }
    }
}
