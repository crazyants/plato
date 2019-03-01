using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.Models;
using Plato.Search.Stores;

namespace Plato.Search.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<SearchResult> _viewProvider;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<SearchResult> viewProvider,
            ISearchSettingsStore<SearchSettings> searchSettingsStore,
            IContextFacade contextFacade)
        {

            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _searchSettingsStore = searchSettingsStore;
            _contextFacade = contextFacade;

            T = localizer;
            S = stringLocalizer;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
        {

            // Default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }
            
            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }
      
            // Get default options
            var defaultViewOptions = new EntityIndexOptions();
            var defaultPagerOptions = new PagerOptions();
            
            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search && !this.RouteData.Values.ContainsKey("opts.search"))
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort && !this.RouteData.Values.ContainsKey("opts.sort"))
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order && !this.RouteData.Values.ContainsKey("opts.order"))
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter && !this.RouteData.Values.ContainsKey("opts.filter"))
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (opts.FeatureId != defaultViewOptions.FeatureId && !this.RouteData.Values.ContainsKey("opts.featureId"))
                this.RouteData.Values.Add("opts.featureId", opts.FeatureId);
            if (opts.Within != defaultViewOptions.Within && !this.RouteData.Values.ContainsKey("opts.within"))
                this.RouteData.Values.Add("opts.within", opts.Within);
            if (pager.Page != defaultPagerOptions.Page && !this.RouteData.Values.ContainsKey("pager.page"))
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize && !this.RouteData.Values.ContainsKey("pager.size"))
                this.RouteData.Values.Add("pager.size", pager.PageSize);
            
            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            this.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetEntities", viewModel);
            }
            
            // Build breadcrumb
            if (string.IsNullOrEmpty(opts.Search))
            {
                _breadCrumbManager.Configure(builder =>
                {
                    builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Search"]);
                });
            }
            else
            {
                _breadCrumbManager.Configure(builder =>
                {
                    builder.Add(S["Home"], home => home
                            .Action("Index", "Home", "Plato.Core")
                            .LocalNav()
                        ).Add(S["Search"], home => home
                            .Action("Index", "Home", "Plato.Search")
                            .LocalNav())
                        .Add(S["Results"]);
                });
            }
            
            // Return view
            return View(await _viewProvider.ProvideIndexAsync(new SearchResult(), this));

        }

        async Task<EntityIndexViewModel<Entity>> GetIndexViewModelAsync(EntityIndexOptions options, PagerOptions pager)
        {

            // Set default sort column if auto is specified
            if (options.Sort == SortBy.Auto)
            {
                // Get search settings
                var searchSettings = await _searchSettingsStore.GetAsync();
                if (searchSettings != null)
                {
                    options.Sort = searchSettings.SearchType == SearchTypes.Tsql
                        ? SortBy.LastReply
                        : SortBy.Rank;
                }
                else
                {
                    options.Sort = SortBy.LastReply;
                }
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(this.RouteData));

            // Return updated model
            return new EntityIndexViewModel<Entity>()
            {
                Options = options,
                Pager = pager
            };

        }


    }

}
