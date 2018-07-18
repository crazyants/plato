using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
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
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }
        
        public HomeController(
            IViewProviderManager<Channel> channelViewProvider,
            IHtmlLocalizer<HomeController> localizer,
            ICategoryStore<Channel> channelStore,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _channelStore = channelStore;
            _channelViewProvider = channelViewProvider;
            _alerter = alerter;
            T = localizer;
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
            
            //this.RouteData.Values.Add("Options.Search", filterOptions.Search);
            //this.RouteData.Values.Add("Options.Order", filterOptions.Order);
            this.RouteData.Values.Add("page", pagerOptions.Page);
    
            // Build view
            var result = await _channelViewProvider.ProvideIndexAsync(category, this);

            // Return view
            return View(result);
            
        }
        
        #endregion
        
    }
    
}
