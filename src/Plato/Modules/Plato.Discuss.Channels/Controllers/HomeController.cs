using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Channels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IViewProviderManager<ChannelViewModel> _channelViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IPostManager<Topic> _postManager;
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }
        
        public HomeController(
            IHtmlLocalizer<HomeController> localizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            IViewProviderManager<ChannelViewModel> channelViewProvider,
            IPostManager<Topic> postManager,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _postManager = postManager;
            _categoryStore = categoryStore;
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

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }

            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
            }
            
            //this.RouteData.Values.Add("Options.Search", filterOptions.Search);
            //this.RouteData.Values.Add("Options.Order", filterOptions.Order);
            this.RouteData.Values.Add("page", pagerOptions.Page);
            this.RouteData.Values.Add("categoryId", id);

            var channelViewModel = new ChannelViewModel()
            {
                Channel = category
            };

            // Build view
            var result = await _channelViewProvider.ProvideIndexAsync(channelViewModel, this);

            // Return view
            return View(result);
            
        }
        
        #endregion
        
    }
    
}
