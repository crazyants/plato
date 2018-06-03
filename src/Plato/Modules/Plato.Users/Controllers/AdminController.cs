using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Layout.TagHelpers;
using Plato.Models.Users;
using Plato.Stores.Users;
using Plato.Users.ViewModels;
using Plato.Navigation;

namespace Plato.Users.Controllers
{

    public class AdminController : Controller
    {
        private readonly IPlatoUserStore<User> _ploatUserStore;

        private readonly IHtmlGenerator _htmlGenertor;

        public AdminController(
            IPlatoUserStore<User> platoUserStore, 
            IHtmlGenerator htmlGenertor)
        {
            _ploatUserStore = platoUserStore;
            _htmlGenertor = htmlGenertor;
         
        }

        public async Task<ActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions
            )
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
            
            return View(await GetModel(filterOptions, pagerOptions));
        }

        private async Task<UsersPaged> GetModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var results = await GetUsers();
            return new UsersPaged()
            {
                Results = results,
                FilterOpts = filterOptions,
                PagerOpts = pagerOptions
            };
        }

        public async Task<IPagedResults<User>> GetUsers()
        {
            return await _ploatUserStore.QueryAsync()
                .Page(1, 20)
                .Select<UserQueryParams>(q =>
                {
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList<User>();
        }


    }
}
