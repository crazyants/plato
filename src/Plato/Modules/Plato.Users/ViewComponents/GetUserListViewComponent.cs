using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{
    public class GetUserListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _ploatUserStore;
        private readonly IUserService _userService;

        public GetUserListViewComponent(
            IContextFacade contextFacade,
            IPlatoUserStore<User> ploatUserStore,
            IUserService userService)
        {
            _contextFacade = contextFacade;
            _ploatUserStore = ploatUserStore;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            UserIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new UserIndexOptions();
            }
            
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            var model = await GetIndexViewModel(options, pager);
            return View(model);
        }
        
        private async Task<UserIndexViewModel> GetIndexViewModel(
            UserIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _userService.GetUsersAsunc(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new UserIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}

