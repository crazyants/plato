using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Follow.Models;
using Plato.Follow.Stores;
using Plato.Follow.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Discuss.Follow.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityFollowStore<EntityFollow> _entityFollowStore;

        public TopicViewProvider(
            IContextFacade contextFacade,
            IEntityFollowStore<EntityFollow> entityFollowStore)
        {
            _contextFacade = contextFacade;
            _entityFollowStore = entityFollowStore;
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
                View<FollowViewModel>("Follow.Entity.Sidebar", model =>
                {
                    model.EntityId = entity.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(10)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Topic entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Topic entity, IUpdateModel updater)
        {

            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Topic entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
