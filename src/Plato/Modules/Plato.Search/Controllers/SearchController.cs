using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.Models;
using Plato.Search.Services;
using Plato.Search.Stores;
using Plato.Search.ViewModels;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Search.Controllers
{

    public class SearchController : BaseWebApiController
    {
        
        private readonly IContextFacade _contextFacade;
        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly IEntityService<Entity> _searchService;

        public SearchController(
            IContextFacade contextFacade,
            ISearchSettingsStore<SearchSettings> searchSettingsStore,
            IEntityService<Entity> searchService)
        {
            _contextFacade = contextFacade;
            _searchSettingsStore = searchSettingsStore;
            _searchService = searchService;
        }
        
        [HttpGet, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            SortBy sort = SortBy.Auto,
            OrderBy order = OrderBy.Desc)
        {

            // Get search settings
            var searchSettings = await _searchSettingsStore.GetAsync();

            // Set default sort column if auto is specified
            if (sort == SortBy.Auto)
            {
                // Get search settings
                if (searchSettings != null)
                {
                    sort = searchSettings.SearchType == SearchTypes.Tsql
                        ? SortBy.LastReply
                        : SortBy.Rank;
                }
                else
                {
                    sort = SortBy.LastReply;
                }
            }
            
            // Get results
            var entities = await _searchService
                .ConfigureDb(o =>
                {
                    if (searchSettings != null)
                    {
                        o.SearchType = searchSettings.SearchType;
                    }
                })
                .GetResultsAsync(new EntityIndexOptions()
                {
                    Search = keywords,
                    Sort = sort,
                    Order = order
                }, new PagerOptions()
                {
                    Page = page,
                    Size = size
                });
            
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
                        ["area"] = "Plato.Discuss",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = entity.Id,
                        ["opts.alias"] = entity.Alias
                    });

                    results.Data.Add(new SearchApiResult()
                    {
                        Id = entity.Id,
                        CreatedBy = new UserApiResult()
                        {
                            Id = entity.CreatedBy.Id,
                            DisplayName = entity.CreatedBy.DisplayName,
                            UserName = entity.CreatedBy.UserName,
                            Avatar = entity.CreatedBy.Avatar,
                            Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Users",
                                ["controller"] = "Home",
                                ["action"] = "Display",
                                ["opts.id"] = entity.CreatedBy.Id,
                                ["opts.alias"] = entity.CreatedBy.Alias
                            })
                        },
                        ModifiedBy = new UserApiResult()
                        {
                            Id = entity.ModifiedBy.Id,
                            DisplayName = entity.ModifiedBy.DisplayName,
                            UserName = entity.ModifiedBy.UserName,
                            Avatar = entity.ModifiedBy.Avatar,
                            Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Users",
                                ["controller"] = "Home",
                                ["action"] = "Display",
                                ["opts.id"] = entity.ModifiedBy.Id,
                                ["opts.alias"] = entity.ModifiedBy.Alias
                            })
                        },
                        LastReplyBy = new UserApiResult()
                        {
                            Id = entity.LastReplyBy.Id,
                            DisplayName = entity.LastReplyBy.DisplayName,
                            UserName = entity.LastReplyBy.UserName,
                            Avatar = entity.LastReplyBy.Avatar,
                            Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Users",
                                ["controller"] = "Home",
                                ["action"] = "Display",
                                ["opts.id"] = entity.LastReplyBy.Id,
                                ["opts.alias"] = entity.LastReplyBy.Alias
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
      
    }

}
