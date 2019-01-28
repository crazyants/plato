using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Discuss.Services
{
    
    public class TopicService : ITopicService
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IPlatoRoleStore _roleStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TopicService(
            IContextFacade contextFacade,
            IEntityStore<Topic> topicStore,
            IFeatureFacade featureFacade,
            IPlatoRoleStore roleStore,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _contextFacade = contextFacade;
            _topicStore = topicStore;
            _featureFacade = featureFacade;
            _roleStore = roleStore;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IPagedResults<Topic>> GetTopicsAsync(TopicIndexOptions options, PagerOptions pager)
        {
            
            if (options == null)
            {
                options = new TopicIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get discuss features
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            var channelFeature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");

            // Get authenticated user for use within view adaptor below
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Get principal
            var principal = _httpContextAccessor.HttpContext.User;

            // Get anonymous role for use within view adapt or below
            var anonymousRole = await _roleStore.GetByNameAsync(DefaultRoles.Anonymous);

            // Return tailored results
            return await _topicStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<EntityQueryParams>(async q => 
                {

                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

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
                    }

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

                    if (options.Params != null)
                    {

                        if (options.Params.ChannelId > 0)
                        {
                            q.CategoryId.Equals(options.Params.ChannelId);
                        }

                        if (options.Params.ChannelIds != null)
                        {
                            q.CategoryId.IsIn(options.Params.ChannelIds);
                        }
                        
                        if (options.Params.LabelId > 0)
                        {
                            q.LabelId.Equals(options.Params.LabelId);
                        }

                        if (options.Params.TagId > 0)
                        {
                            q.TagId.Equals(options.Params.TagId);
                        }

                        if (options.Params.CreatedByUserId > 0)
                        {
                            q.CreatedUserId.Equals(options.Params.CreatedByUserId);
                        }

                    }

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        Permissions.ViewPrivateTopics))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        Permissions.ViewSpamTopics))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        Permissions.ViewDeletedReplies))
                    {
                        q.HideDeleted.True();
                    }
                    
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
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }

    }

}
