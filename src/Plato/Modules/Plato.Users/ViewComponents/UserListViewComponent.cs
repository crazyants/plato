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
            ViewOptions viewOpts,
            PagerOptions pagerOpts)
        {

            if (viewOpts == null)
            {
                viewOpts = new ViewOptions();
            }
            
            if (pagerOpts == null)
            {
                pagerOpts = new PagerOptions();
            }

            var model = await GetIndexViewModel(viewOpts, pagerOpts);
            return View(model);
        }
        
        private async Task<UsersIndexViewModel> GetIndexViewModel(
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            var users = await GetUsers(viewOptions, pagerOptions);
            return new UsersIndexViewModel(
                users,
                viewOptions,
                pagerOptions);
        }

        public async Task<IPagedResults<User>> GetUsers(
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            return await _ploatUserStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!string.IsNullOrEmpty(viewOptions.Search))
                    {
                        q.UserName.StartsWith(viewOptions.Search).Or();
                        q.Email.StartsWith(viewOptions.Search).Or();
                        q.FirstName.StartsWith(viewOptions.Search).Or();
                        q.LastName.StartsWith(viewOptions.Search).Or();
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

