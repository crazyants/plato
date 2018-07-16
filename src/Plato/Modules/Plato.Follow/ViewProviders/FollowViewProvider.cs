using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Follow.Models;
using Plato.Follow.Stores;
using Plato.Follow.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;

namespace Plato.Follow.ViewProviders
{
    //public class FollowViewProvider : BaseViewProvider<Entity>
    //{


    //    private readonly IContextFacade _contextFacade;
    //    private readonly IEntityFollowStore<EntityFollow> entityFollowStore;

    //    public FollowViewProvider(
    //        IContextFacade contextFacade,
    //        IEntityFollowStore<EntityFollow> entityFollowStore)
    //    {
    //        _contextFacade = contextFacade;
    //        this.entityFollowStore = entityFollowStore;
    //    }
        
    //    public override async Task<IViewProviderResult> BuildDisplayAsync(Entity entity, IUpdateModel updater)
    //    {

    //        if (entity == null)
    //        {
    //            await BuildIndexAsync(new Entity(), updater);
    //        }

    //        var isFollowing = false;

    //        var user = await _contextFacade.GetAuthenticatedUserAsync();
    //        if (user != null)
    //        {
    //            var entityFollow = await entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, entity.Id);
    //            if (entityFollow != null)
    //            {
    //                isFollowing = true;
    //            }
    //        }
            
    //        return Views(
    //            View<FollowViewModel>("Follow.Entity.Sidebar", model =>
    //            {
    //                model.EntityId = entity.Id;
    //                model.IsFollowing = isFollowing;
    //                return model;
    //            }).Zone("sidebar").Order(10)
    //        );

    //    }

    //    public override Task<IViewProviderResult> BuildIndexAsync(Entity entity, IUpdateModel updater)
    //    {
    //        return Task.FromResult(default(IViewProviderResult));
    //    }

    //    public override Task<IViewProviderResult> BuildEditAsync(Entity entity, IUpdateModel updater)
    //    {

    //        return Task.FromResult(default(IViewProviderResult));
    //    }

    //    public override Task<IViewProviderResult> BuildUpdateAsync(Entity entity, IUpdateModel updater)
    //    {
    //        return Task.FromResult(default(IViewProviderResult));
    //    }
        
    //}
}
