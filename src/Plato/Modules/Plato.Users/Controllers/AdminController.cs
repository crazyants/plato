using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Stores.Users;
using Plato.Users.ViewModels;

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

        public async Task<ActionResult> Index()
        {
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
