using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Follow;
using Plato.Follow.Models;
using Plato.Follow.Stores;
using Plato.Follow.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Follow.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string FollowHtmlName = "follow";
        
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Plato.Follow.Models.Follow> _followStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly HttpRequest _request;
 
        public TopicViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor,
            IFollowStore<Plato.Follow.Models.Follow> followStore,
            IEntityStore<Topic> entityStore)
        {
            _contextFacade = contextFacade;
            _followStore = followStore;
            _entityStore = entityStore;
            _request = httpContextAccessor.HttpContext.Request;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Topic entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic entity, IViewProviderContext updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Topic(), updater);
            }

            var isFollowing = false;
            var followType = DefaultFollowTypes.Topic;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await _followStore.SelectFollowByNameThingIdAndCreatedUserId(
                    followType.Name,
                    entity.Id,
                    user.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Display.Sidebar", model =>
                {
                    model.FollowType = followType;
                    model.ThingId = entity.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(4)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {
            if (entity == null)
            {
                return await BuildIndexAsync(new Topic(), updater);
            }

            var isFollowing = false;
            var followType = DefaultFollowTypes.Topic;
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await _followStore.SelectFollowByNameThingIdAndCreatedUserId(
                    followType.Name,
                    entity.Id,
                    user.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Edit.Sidebar", model =>
                {
                    model.FollowType = followType;
                    model.FollowHtmlName = FollowHtmlName;
                    model.ThingId = entity.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(2)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext updater)
        {  
            
            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildEditAsync(topic, updater);
            }
    
            // Get the follow checkbox value
            var follow = false;
            foreach (var key in _request.Form.Keys)
            {
                if (key == FollowHtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        follow = true;
                        break;
                    }
                }
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return await BuildEditAsync(topic, updater);
            }

            // Add the follow
            if (follow)
            {
                // Add and return result
                await _followStore.CreateAsync(new Plato.Follow.Models.Follow()
                {
                    ThingId = entity.Id,
                    CreatedUserId = user.Id,
                    CreatedDate = DateTime.UtcNow
                });
            }
            else
            {
                // Delete the follow
                var existingFollow = await _followStore.SelectFollowByNameThingIdAndCreatedUserId(
                        DefaultFollowTypes.Topic.Name,
                        entity.Id,
                        user.Id);
                if (existingFollow != null)
                {
                    await _followStore.DeleteAsync(existingFollow);
                }
            }

            return await BuildEditAsync(topic, updater);

        }

    }
}
