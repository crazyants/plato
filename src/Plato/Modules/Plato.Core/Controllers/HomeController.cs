using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Core.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Core.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
  
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager)
        {
         
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // ---------------------
        // Homepage
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"]);
              
            });
            
            // Build view
            //var result = await _topicViewProvider.ProvideIndexAsync(new Topic(), this);

            // Return view
            return Task.FromResult((IActionResult)View());
            
        }

        // ---------------------
        // Unauthorized / Access Denied
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Denied()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Access Denied"]);
            });
            
            // Return view
            return Task.FromResult((IActionResult)View());

        }
        
        // ---------------------
        // Error page
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
            
            // Build model
            var model = new ErrorViewModel() {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            // Return view
            return Task.FromResult((IActionResult)View(model));
            
        }
        
        #endregion

    }
    
}
