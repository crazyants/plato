using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Entities.Controllers
{


    public class SearchController : BaseWebApiController
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IContextFacade _contextFacade;

        public SearchController(
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            string sort = "LastReplyDate",
            OrderBy order = OrderBy.Desc)
        {

            // Get notificaitons
            var userNotifications = await GetEntities(
                page,
                size,
                keywords,
                sort,
                order);

            IPagedResults<EntityApiResult> results = null;
            if (userNotifications != null)
            {
                results = new PagedResults<EntityApiResult>
                {
                    Total = userNotifications.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var userNotification in userNotifications.Data)
                {

                    var userUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Home",
                        ["Action"] = "Display",
                        ["Id"] = userNotification.CreatedBy.Id,
                        ["Alias"] = userNotification.CreatedBy.Alias
                    });

                    var fromUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Home",
                        ["Action"] = "Display",
                        ["Id"] = userNotification.CreatedBy.Id,
                        ["Alias"] = userNotification.CreatedBy.Alias
                    });

                    var url = "";

                    results.Data.Add(new EntityApiResult()
                    {
                        Id = userNotification.Id,
                        User = new UserApiResult()
                        {
                            Id = userNotification.CreatedBy.Id,
                            DisplayName = userNotification.CreatedBy.DisplayName,
                            UserName = userNotification.CreatedBy.UserName,
                            Url = userUrl
                        },
                        From = new UserApiResult()
                        {
                            Id = userNotification.CreatedBy.Id,
                            DisplayName = userNotification.CreatedBy.DisplayName,
                            UserName = userNotification.CreatedBy.UserName,
                            Url = fromUrl
                        },
                        Title = userNotification.Title,
                        Message = userNotification.Message,
                        Url = url,
                        Date = new FriendlyDate()
                        {
                            Text = userNotification.CreatedDate.ToPrettyDate(),
                            Value = userNotification.CreatedDate
                        }
                    });

                }

            }

            IPagedApiResults<EntityApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<EntityApiResult>()
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


        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region "Private Methods"

        async Task<IPagedResults<Entity>> GetEntities(
            int page,
            int pageSize,
            string keywords,
            string sortBy,
            OrderBy sortOrder)
        {

            return await _entityStore.QueryAsync()
                .Take(page, pageSize)
                .Select<EntityQueryParams>(q =>
                {

                    if (!String.IsNullOrEmpty(keywords))
                    {
                        q.Keywords.Like(keywords);
                    }

                })
                .OrderBy(sortBy, sortOrder)
                .ToList();

        }

        #endregion

    }

}
