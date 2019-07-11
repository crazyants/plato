using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{

    public class GetUserListViewComponent : ViewComponent
    {
        
        private readonly IUserService<User> _userService;

        public GetUserListViewComponent(IUserService<User> userService)
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
                .ConfigureQuery(q =>
                {
                    // We are not within edit mode
                    // Hide spam and banned users
                    if (!options.EnableEdit)
                    {
                        q.HideSpam.True();
                        q.HideBanned.True();
                    }
                })
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

