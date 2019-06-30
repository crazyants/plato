using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Categories.Permissions.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Moderation.Extensions;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Categories.Permissions.ViewComponents
{
    public class ModeratorListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IModeratorStore<Moderator> _moderatorStore;

        public ModeratorListViewComponent(
            IContextFacade contextFacade, 
            IModeratorStore<Moderator> moderatorStore)
        {
            _contextFacade = contextFacade;
            _moderatorStore = moderatorStore;
        }

        #region "Implementation"

        public async Task<IViewComponentResult> InvokeAsync(
            FilterOptions filterOpts,
            PagerOptions pagerOpts)
        {

            if (filterOpts == null)
            {
                filterOpts = new FilterOptions();
            }
            
            if (pagerOpts == null)
            {
                pagerOpts = new PagerOptions();
            }

            return View(new ModeratorIndexViewModel()
            {
                CategorizedModerators = await _moderatorStore.GetCategorizedModeratorsAsync()
            });

        }

        #endregion


    }
    
}

