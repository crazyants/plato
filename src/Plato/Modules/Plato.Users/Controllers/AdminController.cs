using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.Query;
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

        public async Task<ActionResult> Index(
            UsersPagedOptions options,
            PagerOptions pagerOptions
            )
        {

            // default options
            if (options == null)
            {
                options = new UsersPagedOptions();
            }

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                //users = users.Where(u => u.NormalizedUserName.Contains(options.Search) || u.NormalizedEmail.Contains(options.Search));
            }

            switch (options.Order)
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
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);



            return View(await GetModel());
        }

        private async Task<UsersPaged> GetModel()
        {

            var users = await _ploatUserStore.QueryAsync()
                .Page(1, 20)
                .Select<UserQueryParams>(q =>
                {
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList<User>();
            
            return new UsersPaged()
            {
                PagedResults = users
            };

        }
        

    }
}
