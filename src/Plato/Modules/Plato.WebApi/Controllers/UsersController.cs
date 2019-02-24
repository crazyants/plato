using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.WebApi.Models;

namespace Plato.WebApi.Controllers
{
    
    public class UsersController : BaseWebApiController
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IContextFacade _contextFacade;
   
        public UsersController(
            IPlatoUserStore<User> platoUserStore,
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade)
        {
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            string sort = "LastLoginDate",
            OrderBy order = OrderBy.Desc)
        {

            var users = await GetUsers(
                page,
                size,
                keywords,
                sort,
                order);
            
            IPagedResults<UserApiResult> results = null;
            if (users != null)
            {
                results = new PagedResults<UserApiResult>
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

                    results.Data.Add(new UserApiResult()
                    {
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                        Url = profileUrl,
                        Avatar = user.Avatar
                    });
                }
            }

            IPagedApiResults<UserApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<UserApiResult>()
                {
                    Page = page,
                    Size = size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }

        #endregion

        #region "Private Methods"

        async Task<IPagedResults<User>> GetUsers(
            int page,
            int pageSize,
            string username,
            string sortBy,
            OrderBy sortOrder)
        {

            return await _platoUserStore.QueryAsync()
                .Take(page, pageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!String.IsNullOrEmpty(username))
                    {
                        q.Keywords.StartsWith(username);
                    }
                })
                .OrderBy(sortBy, sortOrder)
                .ToList();

        }

        #endregion

    }

}