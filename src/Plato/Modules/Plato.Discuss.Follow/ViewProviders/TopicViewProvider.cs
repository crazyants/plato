using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
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
        private readonly IEntityFollowStore<EntityFollow> _entityFollowStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly HttpRequest _request;
 
        public TopicViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor,
            IEntityFollowStore<EntityFollow> entityFollowStore,
            IEntityStore<Topic> entityStore)
        {
            _contextFacade = contextFacade;
            _entityFollowStore = entityFollowStore;
            _entityStore = entityStore;
            _request = httpContextAccessor.HttpContext.Request;
        }


        public override Task<IViewProviderResult> BuildIndexAsync(Topic entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }


        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic entity, IUpdateModel updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Topic(), updater);
            }

            var isFollowing = false;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, entity.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Display.Sidebar", model =>
                {
                    model.EntityId = entity.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(3)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Topic entity, IUpdateModel updater)
        {
            if (entity == null)
            {
                return await BuildIndexAsync(new Topic(), updater);
            }

            var isFollowing = false;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, entity.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Edit.Sidebar", model =>
                {
                    model.FollowHtmlName = FollowHtmlName;
                    model.EntityId = entity.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(2)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IUpdateModel updater)
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
                await _entityFollowStore.CreateAsync(new EntityFollow()
                {
                    EntityId = entity.Id,
                    UserId = user.Id,
                    CreatedDate = DateTime.UtcNow
                });
            }
            else
            {
                // Delete the follow
                var existingFollow =
                    await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, entity.Id);
                if (existingFollow != null)
                {
                    await _entityFollowStore.DeleteAsync(existingFollow);
                }
            }

            return await BuildEditAsync(topic, updater);

        }

    }
}
