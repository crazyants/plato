using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.History.Models;
using Plato.Entities.History.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Entities.History.Controllers
{

    public class EntityController : BaseWebApiController
    {

        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public EntityController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            IEntityHistoryStore<EntityHistory> entityHistoryStore)
        {
            _contextFacade = contextFacade;
            _entityHistoryStore = entityHistoryStore;

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
            int entityReplyId = 0,
            string sort = "CreatedDate",
            OrderBy order = OrderBy.Desc)
        {

            // Get histories
            var histories = await GetEntityHistory(
                page,
                size,
                entityId,
                entityReplyId,
                sort,
                order);

            IPagedResults<EntityHistoryApiResult> results = null;
            if (histories != null)
            {
                results = new PagedResults<EntityHistoryApiResult>
                {
                    Total = histories.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var history in histories.Data)
                {


                    var historyUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Discuss.History",
                        ["controller"] = "Home",
                        ["action"] = "Index",
                        ["id"] = history.Id
                    });
                    
                    var createdByUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Users",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = history.CreatedBy.Id,
                        ["opts.alias"] = history.CreatedBy.Alias
                    });

                    var sb = new StringBuilder();
                    sb.Append(history.CreatedBy.DisplayName)
                        .Append(" ")
                        .Append(history.MajorVersion == 1 && history.MinorVersion == 0
                            ? T["created"].Value
                            : T["edited"].Value)
                        .Append(" ")
                        .Append(history.CreatedDate.ToPrettyDate());
                        
                    results.Data.Add(new EntityHistoryApiResult()
                    {
                        Id = history.Id,
                        Text = sb.ToString(),
                        Url = historyUrl,
                        Version = history.Version,
                        CreatedBy = new UserApiResult()
                        {
                            Id = history.CreatedBy.Id,
                            DisplayName = history.CreatedBy.DisplayName,
                            UserName = history.CreatedBy.UserName,
                            Avatar = history.CreatedBy.Avatar,
                            Url = createdByUrl
                        },
                        Date = new FriendlyDate()
                        {
                            Text = history.CreatedDate.ToPrettyDate(),
                            Value = history.CreatedDate
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


        #endregion

        #region "Private Methods"

        async Task<IPagedResults<EntityHistory>> GetEntityHistory(
            int page,
            int pageSize,
            int entityId,
            int entityReplyId,
            string sortBy,
            OrderBy sortOrder)
        {
            return await _entityHistoryStore.QueryAsync()
                .Take(page, pageSize)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                    q.EntityReplyId.Equals(entityReplyId);
                })
                .OrderBy(sortBy, sortOrder)
                .ToList();
        }

        #endregion

    }

}
