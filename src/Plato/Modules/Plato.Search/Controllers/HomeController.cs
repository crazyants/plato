using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.Models;
using Plato.Search.Stores;
using Plato.Search.ViewModels;

namespace Plato.Search.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<SearchResult> _viewProvider;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<SearchResult> viewProvider,
            ISearchSettingsStore<SearchSettings> searchSettingsStore)
        {

            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _searchSettingsStore = searchSettingsStore;

            T = localizer;
            S = stringLocalizer;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            SearchIndexOptions opts,
            PagerOptions pager)
        {

            // default options
            if (opts == null)
            {
                opts = new SearchIndexOptions();
            }

            // Set default sort column if auto is specified
            if (opts.Sort == SortBy.Auto)
            {
                // Get search settings
                var searchSettings = await _searchSettingsStore.GetAsync();
                if (searchSettings != null)
                {
                    opts.Sort = searchSettings.SearchType == SearchTypes.Tsql
                        ? SortBy.LastReply
                        : SortBy.Rank;
                }
                else
                {
                    opts.Sort = SortBy.LastReply;
                }
            }
            
            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
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
            
            // Get default options
            var defaultViewOptions = new SearchIndexOptions();
            var defaultPagerOptions = new PagerOptions();
            
            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter)
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            if (opts.FeatureId != 0)
                this.RouteData.Values.Add("opts..featureId", opts.FeatureId);
            if (!string.IsNullOrEmpty(opts.Within))
                this.RouteData.Values.Add("opts..within", opts.Within);

            // Add view options to context for use within view adapters
            this.HttpContext.Items[typeof(SearchIndexViewModel)] = new SearchIndexViewModel()
            {
                Options = opts,
                Pager = pager
            }; 
            
            // Build view
            var result = await _viewProvider.ProvideIndexAsync(new SearchResult(), this);

            // Return view
            return View(result);

        }

    }

}
