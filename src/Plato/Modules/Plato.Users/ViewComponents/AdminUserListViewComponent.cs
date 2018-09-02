using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{
    public class AdminUserListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _ploatUserStore;

        public AdminUserListViewComponent(
            IContextFacade contextFacade,
            IPlatoUserStore<User> ploatUserStore)
        {
            _contextFacade = contextFacade;
            _ploatUserStore = ploatUserStore;
        }

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

            var model = await GetIndexViewModel(filterOpts, pagerOpts);
            return View(model);
        }
        
        private async Task<UsersIndexViewModel> GetIndexViewModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var users = await GetUsers(filterOptions, pagerOptions);
            return new UsersIndexViewModel(
                users,
                filterOptions,
                pagerOptions);
        }

        public async Task<IPagedResults<User>> GetUsers(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _ploatUserStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!string.IsNullOrEmpty(filterOptions.Search))
                    {
                        q.UserName.IsIn(filterOptions.Search).Or();
                        q.Email.IsIn(filterOptions.Search);
                    }
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();
        }


    }


}

