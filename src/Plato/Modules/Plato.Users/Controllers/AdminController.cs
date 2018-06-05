using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Layout.Views;
using Plato.Models.Users;
using Plato.Stores.Users;
using Plato.Users.ViewModels;
using Plato.Navigation;

namespace Plato.Users.Controllers
{

    public class AdminController : Controller
    {
        private readonly IPlatoUserStore<User> _ploatUserStore;
        
        public AdminController(
            IPlatoUserStore<User> platoUserStore)
        {
            _ploatUserStore = platoUserStore;
        }

        public async Task<IActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {




            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }

            if (!string.IsNullOrWhiteSpace(filterOptions.Search))
            {
                //users = users.Where(u => u.NormalizedUserName.Contains(options.Search) || u.NormalizedEmail.Contains(options.Search));
            }

            switch (filterOptions.Order)
            {
                case UsersOrder.Username:
                    //users = users.OrderBy(u => u.NormalizedUserName);
                    break;
                case UsersOrder.Email:
                    //users = users.OrderBy(u => u.NormalizedEmail);
                    break;
            }
            
            // Maintain previous route data when generating page links
            var routeData = new RouteData();
            //routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", filterOptions.Search);
            routeData.Values.Add("Options.Order", filterOptions.Order);


            var model = await GetModel(filterOptions, pagerOptions);
            model.View = new View("UserList", model);

            return View(model);
        }

        private async Task<UsersPaged> GetModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return new UsersPaged()
            {
                Results = await GetUsers(filterOptions, pagerOptions),
                FilterOpts = filterOptions,
                PagerOpts = pagerOptions,
            };
        }

        public async Task<IPagedResults<User>> GetUsers(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _ploatUserStore.QueryAsync()
                .Page(pagerOptions.Page, pagerOptions.PageSize)
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
                .OrderBy("Id", OrderBy.Asc)
                .ToList<User>();
        }


    }
}
