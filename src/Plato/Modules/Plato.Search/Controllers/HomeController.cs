using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Shell.Abstractions;
using Plato.Search.ViewModels;
using Plato.WebApi.Controllers;

namespace Plato.Search.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<SearchResult> _viewProvider;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<SearchResult> viewProvider)
        {

            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {

            // default options
            if (viewOptions == null)
            {
                viewOptions = new ViewOptions();
            }

            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
            }

            // Build breadcrumb
            if (string.IsNullOrEmpty(viewOptions.Search))
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


            this.RouteData.Values.Add("search", viewOptions.Search);
            this.RouteData.Values.Add("page", pagerOptions.Page);

            // Build view
            var result = await _viewProvider.ProvideIndexAsync(new SearchResult(), this);

            // Return view
            return View(result);

        }

        #endregion

    }

}
