using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.Localizers;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.Tasks;
using Plato.Internal.Layout.Theming;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoViewAdapters(
            this IServiceCollection services)
        {
            // view adapters
            services.TryAddScoped<IViewAdapterManager, ViewAdapterManager>();

            return services;

        }

        public static IServiceCollection AddPlatoViewFeature(
            this IServiceCollection services)
        {

            // Layout updater
            services.AddSingleton<ILayoutUpdater, LayoutUpdater>();
            
            // Generic views
            services.AddSingleton<IViewHelperFactory, ViewDisplayHelperFactory>();
            services.AddSingleton<IViewFactory, ViewFactory>();
            services.AddSingleton<IViewTableManager, ViewTableManager>();
            services.AddSingleton<IViewInvoker, ViewInvoker>();

            // Add page title builder
            services.AddScoped<IPageTitleBuilder, PageTitleBuilder>();
            
            // Add theming conventions - configures theme layout based on controller prefix
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            // Action filters
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(typeof(ModelBinderAccessorFilter));
                options.Filters.Add(typeof(AlertFilter));
                //options.Filters.Add(typeof(DeferredTasksFilter));
                options.Filters.Add(typeof(ModularFilter));
            });

            // Model binding model accessor
            services.AddScoped<IUpdateModelAccessor, LocalModelBinderAccessor>();

            // Alerter
            services.AddScoped<IAlerter, Alerter>();
            
            return services;

        }

        public static IServiceCollection AddPlatoViewLocalization(
            this IServiceCollection services)
        {

            // Localization
            services.AddScoped<IStringLocalizer, LocaleStringLocalizer>();
            services.AddScoped<IHtmlLocalizer, LocaleHtmlLocalizer>();

            // View localization
            services.AddScoped<IViewLocalizer, LocaleViewLocalizer>();

            return services;

        }

    }
}
