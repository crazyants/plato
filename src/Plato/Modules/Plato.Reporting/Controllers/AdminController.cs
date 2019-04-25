using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Admin.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Metrics.Models;
using Plato.Reporting.Models;
using Plato.Reporting.ViewModels;

namespace Plato.Reporting.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IViewProviderManager<Report> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IViewProviderManager<Report> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter, 
            IFeatureFacade featureFacade)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _featureFacade = featureFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index(ReportIndexOptions opts, PagerOptions pager)
        {

            // Default options
            if (opts == null)
            {
                opts = new ReportIndexOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }


            // Get default options
            var defaultViewOptions = new ReportIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(ReportIndexViewModel<Metric>)] = viewModel;

            // If we have a pager.page querystring value return paged view
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetMetrics", viewModel);
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Reports"], reports => reports
                        .LocalNav());
            });

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new Report(), this));
            
        }

        //[HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        //public async Task<IActionResult> IndexPost()
        //{

        //    // Execute view providers
        //    await _viewProvider.ProvideUpdateAsync(new Report(), this);

        //    if (!ModelState.IsValid)
        //    {

        //        // if we reach this point some view model validation
        //        // failed within a view provider, display model state errors
        //        foreach (var modelState in ViewData.ModelState.Values)
        //        {
        //            foreach (var error in modelState.Errors)
        //            {
        //                _alerter.Danger(T[error.ErrorMessage]);
        //            }
        //        }

        //    }

        //    return await Index();

        //}

        async Task<ReportIndexViewModel<Metric>> GetIndexViewModelAsync(ReportIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            //// Ensure we have a default sort column
            //if (options.Sort == SortBy.Auto)
            //{
            //    options.Sort = SortBy.LastReply;
            //}

            // Return updated model
            return new ReportIndexViewModel<Metric>()
            {
                Options = options,
                Pager = pager
            };

        }


    }

}
