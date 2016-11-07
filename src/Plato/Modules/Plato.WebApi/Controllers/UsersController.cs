using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Plato.Stores.Users;
using Plato.Models.Users;

namespace Plato.WebApi.Controllers
{



    public class UsersController : Controller
    {

        private readonly IPlatoUserStore _userStore;

        public UsersController(
            IPlatoUserStore userStore
        )
        {
            _userStore = userStore;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var users = await _userStore.QueryAsync()
                .Page(1, 5)
                .Select<UserQueryParams>(q =>
                {
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);

                })
                .OrderBy("Id")
                .ToList<User>();


            return new ObjectResult(new
            {
                Data = users,
                StatusCode = HttpStatusCode.OK,
                Message = "Album created successfully."
            });

        }


    }
}
