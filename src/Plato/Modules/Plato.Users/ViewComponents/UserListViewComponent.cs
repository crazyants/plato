using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{
    public class UserListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _ploatUserStore;

        public UserListViewComponent(
            IContextFacade contextFacade,
            IPlatoUserStore<User> ploatUserStore)
        {
            _contextFacade = contextFacade;
            _ploatUserStore = ploatUserStore;
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
            UserIndexOptions viewOptions,
            PagerOptions pagerOptions)
        {
            var users = await GetUsers(viewOptions, pagerOptions);
            return new UserIndexViewModel(
                users,
                viewOptions,
                pagerOptions);
        }

        public async Task<IPagedResults<User>> GetUsers(
            UserIndexOptions viewOptions,
            PagerOptions pagerOptions)
        {
            return await _ploatUserStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!string.IsNullOrEmpty(viewOptions.Search))
                    {
                        q.Keywords.Like(viewOptions.Search);
                    }
                })
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();
        }


    }


}

