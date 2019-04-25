using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Core.Models;
using Plato.Core.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Core.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<HomeIndex> _viewProvider;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,
            IContextFacade contextFacade,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<HomeIndex> viewProvider)
        {

            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // ---------------------
        // Homepage
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    );
            });

            // Return view
            return View((LayoutViewModel)await _viewProvider.ProvideIndexAsync(new HomeIndex(), this));

        }

        // ---------------------
        // Unauthorized - 401
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Denied(string returnUrl = null)
        {

            // Persist returnUrl
            ViewData["ReturnUrl"] = returnUrl;

            Response.StatusCode = 401;

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Access Denied"]);
            });

            // Return view
            return Task.FromResult((IActionResult) View());

        }
        
        // ---------------------
        // Moved - 404
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Moved()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Not Found"]);
            });

            Response.StatusCode = 404;

            // Build model
            var model = new MovedViewModel()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            // Return view
            return Task.FromResult((IActionResult) View(model));

        }

        // ---------------------
        // Error - 500
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Error()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Error"]);
            });

            Response.StatusCode = 500;

            // Build model
            var model = new ErrorViewModel()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            // Return view
            return Task.FromResult((IActionResult)View(model));

        }
        
        #endregion

    }

}
