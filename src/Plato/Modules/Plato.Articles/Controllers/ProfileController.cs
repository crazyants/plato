using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Settings;
using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Articles.Controllers
{
    public class ProfileController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IViewProviderManager<DiscussUser> _viewProvider;

        public ProfileController(
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IViewProviderManager<DiscussUser> viewProvider)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
        }
        
        public async Task<IActionResult> Index(
            int id,
            ArticleIndexOptions opts,
            PagerOptions pager)
        {

            if (id <= 0)
            {
                return NotFound();
            }

            // default options
            if (opts == null)
            {
                opts = new ArticleIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get default options
            var defaultViewOptions = new ArticleIndexOptions();
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

            // Add view options to context for use within view adaptors
            this.HttpContext.Items[typeof(ArticleIndexViewModel)] = new ArticleIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Build breadcr

            // Build view
            var result = await _viewProvider.ProvideDisplayAsync(new DiscussUser()
            {
                Id = id
            }, this);

            //// Return view
            return View(result);

            //return Task.FromResult((IActionResult)View());
        }

        

    }

}
