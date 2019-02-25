using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Search.Models;
using Plato.Search.Stores;
using Plato.Search.ViewModels;

namespace Plato.Search.Services
{
    
    public class SearchService : ISearchService
    {
        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlatoRoleStore _roleStore;

        public SearchService(
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore,
            IFeatureFacade featureFacade,
            ISearchSettingsStore<SearchSettings> searchSettingsStore,
            IAuthorizationService authorizationService, 
            IHttpContextAccessor httpContextAccessor,
            IPlatoRoleStore roleStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _featureFacade = featureFacade;
            _searchSettingsStore = searchSettingsStore;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _roleStore = roleStore;
        }

        public async Task<IPagedResults<Entity>> GetResultsAsync(SearchIndexOptions options, PagerOptions pager)
        {
            
            if (options == null)
            {
                options = new SearchIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            // Get search settings
            var searchSettings = await _searchSettingsStore.GetAsync();

            // If full text is enabled ensure we sort by rank by default
            if (searchSettings != null)
            {
                if (searchSettings.SearchType != SearchTypes.Tsql)
                {
                    options.Sort = SortBy.Rank;
                }
            }
            
            // Get authenticated user 
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Get principal
            var principal = _httpContextAccessor.HttpContext.User;
            
            // Return tailored results
            return await _entityStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Configure(o =>
                {
                    if (searchSettings != null)
                    {
                        o.SearchType = searchSettings.SearchType;
                    }
                })
                .Select<EntityQueryParams>(q =>
                {
                    
                    // Filters
                    switch (options.Filter)
                    {
                        case FilterBy.MyTopics:
                            if (user != null)
                            {
                                q.CreatedUserId.Equals(user.Id);
                            }

                            break;
                        case FilterBy.Participated:
                            if (user != null)
                            {
                                q.ParticipatedUserId.Equals(user.Id);
                            }

                            break;
                        case FilterBy.Following:
                            if (user != null)
                            {
                                q.FollowUserId.Equals(user.Id, b =>
                                {
                                    // Restrict follows by topic
                                    b.Append(" AND f.[Name] = 'Topic'");
                                });
                            }

                            break;
                        case FilterBy.Starred:
                            if (user != null)
                            {
                                q.StarUserId.Equals(user.Id, b =>
                                {
                                    // Restrict follows by topic
                                    b.Append(" AND s.[Name] = 'Topic'");
                                });
                            }

                            break;

                        case FilterBy.NoReplies:

                            q.TotalReplies.Equals(0);
                            break;
                    }

                    // Keywords
                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }

                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
                    
                    // Restrict results via user role if the channels feature is enabled
                    //if (channelFeature != null)
                    //{
                    //    if (user != null)
                    //    {
                    //        q.RoleId.IsIn(user.UserRoles?.Select(r => r.Id).ToArray());
                    //    }
                    //    else
                    //    {
                    //        if (anonymousRole != null)
                    //        {
                    //            q.RoleId.Equals(anonymousRole.Id);
                    //        }
                    //    }
                    //}

                    //if (options.Params != null)
                    //{

                    //    if (options.Params.ChannelId > 0)
                    //    {
                    //        q.CategoryId.Equals(options.Params.ChannelId);
                    //    }

                    //    if (options.Params.ChannelIds != null)
                    //    {
                    //        q.CategoryId.IsIn(options.Params.ChannelIds);
                    //    }

                    //    if (options.Params.LabelId > 0)
                    //    {
                    //        q.LabelId.Equals(options.Params.LabelId);
                    //    }

                    //    if (options.Params.TagId > 0)
                    //    {
                    //        q.TagId.Equals(options.Params.TagId);
                    //    }

                    //    if (options.Params.CreatedByUserId > 0)
                    //    {
                    //        q.CreatedUserId.Equals(options.Params.CreatedByUserId);
                    //    }

                    //}

                    //// Hide private?
                    //if (!await _authorizationService.AuthorizeAsync(principal,
                    //    Permissions.ViewPrivateTopics))
                    //{
                    //    q.HidePrivate.True();
                    //}

                    //// Hide spam?
                    //if (!await _authorizationService.AuthorizeAsync(principal,
                    //    Permissions.ViewSpamTopics))
                    //{
                    //    q.HideSpam.True();
                    //}

                    //// Hide deleted?
                    //if (!await _authorizationService.AuthorizeAsync(principal,
                    //    Permissions.ViewDeletedTopics))
                    //{
                    //    q.HideDeleted.True();
                    //}

                    //q.IsPinned.True();
                    //if (!string.IsNullOrEmpty(filterOptions.Search))
                    //{
                    //    q.UserName.IsIn(filterOptions.Search).Or();
                    //    q.Email.IsIn(filterOptions.Search);
                    //}
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);

                })
                .OrderBy("IsPinned", OrderBy.Desc)
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();



        }
    }
}
