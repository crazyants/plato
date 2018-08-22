using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;

namespace Plato.WebApi.Controllers
{

    public class SearchResult
    {
        public string Text { get; set; }

        public string Url { get; set; }

        public int Rank { get; set; }

    }

    public class UsersController : BaseWebApiController
    {
        private readonly IPlatoUserStore<User> _ploatUserStore;

        public UsersController(
            IPlatoUserStore<User> platoUserStore)
        {
            _ploatUserStore = platoUserStore;
        }


        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Search(
            int page = 1,
            int pageSize = 10,
            string sortBy = "Id",
            string username = "",
            OrderBy sortOrder = OrderBy.Asc)
        {

            var users = await _ploatUserStore.QueryAsync()
                .Take(page, pageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!String.IsNullOrEmpty(username))
                    {
                        q.UserName.Equals(username).Or();
                        //q.FirstName.Equals(username).Or();
                        //q.LastName.Equals(username).Or();
                        //q.DisplayName.Equals(username).Or();
                    }
                })
                .OrderBy(sortBy, sortOrder)
                .ToList();


            PagedResults<SearchResult> output = null;
            if (users != null)
            {
                output = new PagedResults<SearchResult>
                {
                    Total = users.Total
                };
                foreach (var user in users.Data)
                {
                    output.Data.Add(new SearchResult()
                    {
                        Text = user.DisplayName,
                        Url = "http://test.com/",
                        Rank = 0
                    });
                }
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }
        
        [Authorize(Policy = "ManageStore"), HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1, 
            int pageSize = 10,
            string sortBy = "Id",
            string username = "",
            OrderBy sortOrder = OrderBy.Asc)
        {

            var users = await _ploatUserStore.QueryAsync()
                .Take(page, pageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!String.IsNullOrEmpty(username))
                    {
                        q.UserName.Equals(username).Or();
                        //q.FirstName.Equals(username).Or();
                        //q.LastName.Equals(username).Or();
                        //q.DisplayName.Equals(username).Or();
                    }
                })
                .OrderBy(sortBy, sortOrder)
                .ToList();
            
            return users != null
                ? base.Result(users)
                : base.NoResults();

        }

    }
}