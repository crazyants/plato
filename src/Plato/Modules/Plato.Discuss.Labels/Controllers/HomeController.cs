using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Discuss.Labels.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Stores;
using Plato.Discuss.Labels.ViewModels;

namespace Plato.Discuss.Labels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IViewProviderManager<Label> _labelViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ILabelStore<Label> _channelStore;
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }
        
        public HomeController(
            IViewProviderManager<Label> labelViewProvider,
            IHtmlLocalizer<HomeController> localizer,
            ILabelStore<Label> channelStore,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _channelStore = channelStore;
            _labelViewProvider = labelViewProvider;
            _alerter = alerter;
            T = localizer;
        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(
            int id,
            ViewOptions viewOptions,
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
            var result = await _labelViewProvider.ProvideIndexAsync(category, this);

            // Return view
            return View(result);
            
        }
        
        #endregion
        
    }
    
}
