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
using Plato.Search.Models;
using Plato.Search.Stores;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Search.Controllers
{


    public class SearchController : BaseWebApiController
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;

        public SearchController(
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore,
            ISearchSettingsStore<SearchSettings> searchSettingsStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _searchSettingsStore = searchSettingsStore;
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
            var entities = await GetEntities(
                page,
                size,
                keywords,
                sort,
                order);

            IPagedResults<SearchApiResult> results = null;
            if (entities != null)
            {
                results = new PagedResults<SearchApiResult>
                {
                    Total = entities.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var entity in entities.Data)
                {

                    var url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Discuss",
                        ["Controller"] = "Home",
                        ["Action"] = "Topic",
                        ["Id"] = entity.Id,
                        ["Alias"] = entity.Alias
                    });

                    results.Data.Add(new SearchApiResult()
                    {
                        Id = entity.Id,
                        CreatedBy = new UserApiResult()
                        {
                            Id = entity.CreatedBy.Id,
                            DisplayName = entity.CreatedBy.DisplayName,
                            UserName = entity.CreatedBy.UserName,
                            Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["Area"] = "Plato.Users",
                                ["Controller"] = "Home",
                                ["Action"] = "Display",
                                ["Id"] = entity.CreatedBy.Id,
                                ["Alias"] = entity.CreatedBy.Alias
                            })
                        },
                        ModifiedBy = new UserApiResult()
                        {
                            Id = entity.ModifiedBy.Id,
                            DisplayName = entity.ModifiedBy.DisplayName,
                            UserName = entity.ModifiedBy.UserName,
                            Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["Area"] = "Plato.Users",
                                ["Controller"] = "Home",
                                ["Action"] = "Display",
                                ["Id"] = entity.ModifiedBy.Id,
                                ["Alias"] = entity.ModifiedBy.Alias
                            })
                        },
                        LastReplyBy = new UserApiResult()
                        {
                            Id = entity.LastReplyBy.Id,
                            DisplayName = entity.LastReplyBy.DisplayName,
                            UserName = entity.LastReplyBy.UserName,
                            Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["Area"] = "Plato.Users",
                                ["Controller"] = "Home",
                                ["Action"] = "Display",
                                ["Id"] = entity.LastReplyBy.Id,
                                ["Alias"] = entity.LastReplyBy.Alias
                            })
                        },
                        Title = entity.Title,
                        Excerpt = entity.Abstract,
                        Url = url,
                        CreatedDate = new FriendlyDate()
                        {
                            Text = entity.CreatedDate.ToPrettyDate(),
                            Value = entity.CreatedDate
                        },
                        ModifiedDate = new FriendlyDate()
                        {
                            Text = entity.ModifiedDate.ToPrettyDate(),
                            Value = entity.ModifiedDate
                        },
                        LastReplyDate = new FriendlyDate()
                        {
                            Text = entity.LastReplyDate.ToPrettyDate(),
                            Value = entity.LastReplyDate
                        },
                        Relevance = entity.Relevance
                    });

                }

            }

            IPagedApiResults<SearchApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<SearchApiResult>()
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

            var searchSettings = await _searchSettingsStore.GetAsync();
            return await _entityStore.QueryAsync()
                .Take(page, pageSize)
                .Configure(opts =>
                {
                    if (searchSettings != null)
                    {
                        opts.SearchType = searchSettings.SearchType;
                    }
                })
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
