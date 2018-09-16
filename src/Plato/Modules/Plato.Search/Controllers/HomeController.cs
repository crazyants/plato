using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Shell.Abstractions;
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

        public async Task<IActionResult> Index()
        {
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Search")
                    .LocalNav()
                ).Add(S["Search"]);
              
            });


            // Build view
            //var result = await _topicViewProvider.ProvideIndexAsync(new Topic(), this);

            // Build view
            var result = await _viewProvider.ProvideIndexAsync(new SearchResult(), this);

            // Return view
            return View(result);

        }
        
        #endregion
        
    }
    
}
