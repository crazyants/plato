using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Entities.Follow.ViewModels;
using Plato.Entities.Models;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Follow.ViewProviders
{
    public class FollowViewProvider : BaseViewProvider<Entity>
    {

     
        public FollowViewProvider()
        {
         
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Entity entity, IUpdateModel updater)
        {
            return Task.FromResult(Views(
                View<FollowViewModel>("Follow.Entity.Sidebar", model =>
                {
                    model.IsFollowing = true;
                    return model;
                }).Zone("sidebar").Order(10)
            ));

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

            //var model = new EditSocialViewModel();

            //if (!await updater.TryUpdateModelAsync(model))
            //{
            //    return await BuildEditAsync(user, updater);
            //}

            ////model.FacebookUrl = model.UserName?.Trim();

            //if (updater.ModelState.IsValid)
            //{
            //    var result = await _socialLinksStore.UpdateAsync(user.Id, new SocialLinks()
            //    {
            //        FacebookUrl = model.FacebookUrl,
            //        TwitterUrl  = model.TwitterUrl,
            //        YouTubeUrl = model.YouTubeUrl
            //    });
            //}

            //return await BuildEditAsync(user, updater);

            return Task.FromResult(default(IViewProviderResult));

        }
        
    }
}
