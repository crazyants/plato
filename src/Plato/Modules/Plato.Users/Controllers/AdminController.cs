using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Layout.Drivers;
using Plato.Layout.Views;
using Plato.Models.Users;
using Plato.Stores.Users;
using Plato.Users.ViewModels;
using Plato.Navigation;
using Plato.Layout.ModelBinding;

namespace Plato.Users.Controllers
{
    
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<User> _viewProviderManager;
        private readonly IPlatoUserStore<User> _ploatUserStore;
        
        public AdminController(
        
            IPlatoUserStore<User> platoUserStore, IViewProviderManager<User> viewProviderManager)
        {
            _ploatUserStore = platoUserStore;
            _viewProviderManager = viewProviderManager;
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
            var view = new GenericView("UserList", model);
            
            
            var views = new List<GenericView>
            {
                new GenericView("UserTest1", model),
                new GenericView("UserTest2", model)
            };

            var user = new User();
            user.UserName = "Ryan Healey";
            user.Email = "sales@instantasp.co.uk";

       
            var providedView = _viewProviderManager.BuildDisplayAsync(user, this);

            return View(views);

        }

        private async Task<UsersPagedViewModel> GetModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var users = await GetUsers(filterOptions, pagerOptions);
            return new UsersPagedViewModel(
                users,
                filterOptions,
                pagerOptions);
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
