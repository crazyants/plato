using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Entities.Follow.Models;
using Plato.Entities.Follow.Stores;
using Plato.Entities.Follow.ViewModels;
using Plato.Entities.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Follow.ViewProviders
{
    public class FollowViewProvider : BaseViewProvider<Entity>
    {


        private readonly IContextFacade _contextFacade;
        private readonly IEntityFollowStore<EntityFollow> entityFollowStore;

        public FollowViewProvider(
            IContextFacade contextFacade,
            IEntityFollowStore<EntityFollow> entityFollowStore)
        {
            _contextFacade = contextFacade;
            this.entityFollowStore = entityFollowStore;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Entity entity, IUpdateModel updater)
        {
            var isFollowing = false;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, entity.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Entity.Sidebar", model =>
                {
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("sidebar").Order(10)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Entity entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Entity entity, IUpdateModel updater)
        {

            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Entity entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
