using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
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
        private readonly IContextFacade _contextFacade;
   
        public UsersController(
            IPlatoUserStore<User> platoUserStore,
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade)
        {
            _ploatUserStore = platoUserStore;
            _contextFacade = contextFacade;
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
                
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var user in users.Data)
                {

                    var profileUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Home",
                        ["Action"] = "Display",
                        ["Id"] = user.Id,
                        ["Alias"] = user.Alias
                    });

                    output.Data.Add(new SearchResult()
                    {
                        Text = user.DisplayName,
                        Url = profileUrl,
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
                        q.UserName.StartsWith(username).Or();
                        q.Email.StartsWith(username).Or();
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