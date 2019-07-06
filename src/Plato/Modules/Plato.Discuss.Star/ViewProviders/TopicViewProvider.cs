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
        
        private readonly IStarStore<Stars.Models.Star> _starStore;
        private readonly IContextFacade _contextFacade;
       
        public TopicViewProvider(
            IStarStore<Stars.Models.Star> starStore,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            _starStore = starStore;
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

            var isStarred = false;
            var starType = StarTypes.Topic;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityStar = await _starStore.SelectByNameThingIdAndCreatedUserId(
                    starType.Name,
                    entity.Id,
                    user.Id);
                if (entityStar != null)
                {
                    isStarred = true;
                }
            }
            
            return Views(
                View<StarViewModel>("Star.Display.Tools", model =>
                {
                    model.StarType = starType;
                    model.ThingId = entity.Id;
                    model.IsStarred = isStarred;
                    model.TotalStars = entity.TotalStars;
                    model.Permission = Permissions.StarTopics;
                    return model;
                }).Zone("tools").Order(-5)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Topic model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
