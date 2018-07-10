using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Social.Models;
using Plato.Users.Social.Stores;
using Plato.Users.Social.ViewModels;

namespace Plato.Users.Social.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
 
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            UserManager<User> userManager, IPlatoUserStore<User> platoUserStore)
        {
            _userManager = userManager;
            _platoUserStore = platoUserStore;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {

            var socialLinks = user.TryGet<SocialLinks>();

            return Views(
                View<EditSocialViewModel>("Social.Edit.Content", model =>
                {
                    model.FacebookUrl = socialLinks?.FacebookUrl;
                    return model;
                }).Order(10)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditSocialViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }
            
            //model.FacebookUrl = model.UserName?.Trim();

            if (updater.ModelState.IsValid)
            {

                var socialLinks = new SocialLinks()
                {
                    FacebookUrl = model.FacebookUrl,
                    TwitterUrl = model.TwitterUrl,
                    YouTubeUrl = model.YouTubeUrl
                };

                user.AddOrUpdate<SocialLinks>(socialLinks);

                var result = await _platoUserStore.UpdateAsync(user);
            }
            
            return await BuildEditAsync(user, updater);

        }
        
    }
}
