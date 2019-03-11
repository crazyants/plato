using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Navigation.Abstractions;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{

    public class GetUserListViewComponent : ViewComponent
    {
        
        private readonly IUserService _userService;

        public GetUserListViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(UserIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new UserIndexOptions();
            }
            
            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetIndexViewModel(options, pager));

        }
        
        private async Task<UserIndexViewModel> GetIndexViewModel(UserIndexOptions options, PagerOptions pager)
        {

            // Get results
            var results = await _userService
                .GetResultsAsync(options, pager);

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

