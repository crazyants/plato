using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Discuss.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IViewProviderManager<Channel> _channelViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IViewProviderManager<Channel> channelViewProvider,
            IStringLocalizer<AdminController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IBreadCrumbManager breadCrumbManager,
            ICategoryStore<Channel> channelStore,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _channelStore = channelStore;
            _channelViewProvider = channelViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
          
            T = localizer;
            S = stringLocalizer;
        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(
            int id,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            var category = await _channelStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check permissions

            // Build breadcrumb
            var parents = await _channelStore.GetParentsByIdAsync(id);
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                );

                if (parents == null)
                {
                    builder.Add(S["Discuss"]);
                }
                else
                {
                    builder.Add(S["Discuss"], channels => channels
                        .Action("Index", "Home", "Plato.Discuss", new RouteValueDictionary { ["Id"] = 0 })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Admin", "Plato.Discuss.Channels", new RouteValueDictionary { ["Id"] = parent.Id })
                            .LocalNav()
                        );
                    }
                }
                
            });
            
            // Add route data for view provider
            this.RouteData.Values.Add("page", pagerOptions.Page);
    
            // Build view
            var result = await _channelViewProvider.ProvideIndexAsync(category, this);

            // Return view
            return View(result);
            
        }
        
        #endregion
        
    }
    
}
