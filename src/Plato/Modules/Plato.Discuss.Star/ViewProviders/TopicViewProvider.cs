using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Stars.Services;
using Plato.Stars.Stores;
using Plato.Stars.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Star.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string StarHtmlName = "star";
        
        private readonly IContextFacade _contextFacade;
        private readonly IStarStore<Stars.Models.Star> _starStore;
        private readonly IStarManager<Stars.Models.Star> _starManager;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly HttpRequest _request;
 
        public TopicViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor,
            IStarStore<Stars.Models.Star> starStore,
            IEntityStore<Topic> entityStore,
            IStarManager<Stars.Models.Star> starManager)
        {
            _contextFacade = contextFacade;
            _starStore = starStore;
            _entityStore = entityStore;
            _starManager = starManager;
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
            var followType = StarTypes.Topic;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityStar = await _starStore.SelectByNameThingIdAndCreatedUserId(
                    followType.Name,
                    entity.Id,
                    user.Id);
                if (entityStar != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<StarViewModel>("Star.Display.Tools", model =>
                {
                    model.StarType = followType;
                    model.ThingId = entity.Id;
                    model.IsFollowing = isFollowing;
                    model.TotalStars = entity.TotalStars;
                    return model;
                }).Zone("tools").Order(int.MinValue)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
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
                if (key == StarHtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        follow = true;
                        break;
                    }
                }
            }

            // We need to be authenticated to follow
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return await BuildEditAsync(topic, updater);
            }

            // The follow type
            var followType = StarTypes.Topic;
      
            // Get any existing follow
            var existingFollow = await _starStore.SelectByNameThingIdAndCreatedUserId(
                followType.Name,
                entity.Id,
                user.Id);
            
            // Add the follow
            if (follow)
            {
                // If we didn't find an existing follow create a new one
                if (existingFollow == null)
                {
                    // Add follow
                    await _starManager.CreateAsync(new Stars.Models.Star()
                    {
                        Name = followType.Name,
                        ThingId = entity.Id,
                        CreatedUserId = user.Id,
                        CreatedDate = DateTime.UtcNow
                    });
                }
      
            }
            else
            {
                if (existingFollow != null)
                {
                    await _starManager.DeleteAsync(existingFollow);
                }
            }

            return await BuildEditAsync(topic, updater);

        }

    }
}
