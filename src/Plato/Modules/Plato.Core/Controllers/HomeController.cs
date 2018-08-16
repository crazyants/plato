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
        
        #endregion
        
    }
    
}
