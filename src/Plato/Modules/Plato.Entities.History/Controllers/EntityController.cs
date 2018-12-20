using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.History.Models;
using Plato.Entities.History.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Entities.History.Controllers
{
    
    public class EntityController : BaseWebApiController
    {

        private readonly IEntityHistoryStore<EntityHistory> _userNotificationStore;
        private readonly IPlatoUserStore<User> _ploatUserStore;
        private readonly IContextFacade _contextFacade;
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public EntityController(
            IPlatoUserStore<User> platoUserStore,
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade,
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IEntityHistoryStore<EntityHistory> userNotificationStore)
        {
            _ploatUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _userNotificationStore = userNotificationStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            int entityId = 0,
            string sort = "CreatedDate",
            OrderBy order = OrderBy.Desc)
        {

         // Get entity history
            var userNotifications = await GetEntityHistory(
                page,
                size,
                entityId,
                sort,
                order);

            IPagedResults<EntityHistoryApiResult> results = null;
            if (userNotifications != null)
            {
                results = new PagedResults<EntityHistoryApiResult>
                {
                    Total = userNotifications.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var userNotification in userNotifications.Data)
                {
                    
                    var createdByUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Users",
                        ["Controller"] = "Home",
                        ["Action"] = "Display",
                        ["Id"] = userNotification.CreatedBy.Id,
                        ["Alias"] = userNotification.CreatedBy.Alias
                    });

                    var sb = new StringBuilder();
                    sb.Append(userNotification.CreatedBy.DisplayName)
                        .Append(" ")
                        .Append(T["edited"].Value)
                        .Append(" ")
                        .Append(userNotification.CreatedDate.ToPrettyDate());
                        
                    results.Data.Add(new EntityHistoryApiResult()
                    {
                        Id = userNotification.Id,
                        Text = sb.ToString(),
                        CreatedBy = new UserApiResult()
                        {
                            Id = userNotification.CreatedBy.Id,
                            DisplayName = userNotification.CreatedBy.DisplayName,
                            UserName = userNotification.CreatedBy.UserName,
                            Url = createdByUrl
                        },
                   
                        Date = new FriendlyDate()
                        {
                            Text = userNotification.CreatedDate.ToPrettyDate(),
                            Value = userNotification.CreatedDate
                        }
                    });

                }

            }

            IPagedApiResults<EntityHistoryApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<EntityHistoryApiResult>()
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
        public async Task<IActionResult> Delete(int id)
        {
        
            return base.InternalServerError();

        }
        
        #endregion

        #region "Private Methods"

        async Task<IPagedResults<EntityHistory>> GetEntityHistory(
            int page,
            int pageSize,
            int entityId,
            string sortBy,
            OrderBy sortOrder)
        {
            return await _userNotificationStore.QueryAsync()
                .Take(page, pageSize)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    if (entityId > 0)
                    {
                        q.EntityId.Equals(entityId);
                    }
                    
                })
                .OrderBy(sortBy, sortOrder)
                .ToList();
        }

        #endregion

    }

}
