using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Social.Models;
using Plato.Users.Social.ViewModels;

namespace Plato.Users.Social.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(IPlatoUserStore<User> platoUserStore)
        {
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

        public override Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {

            var socialLinks = user.GetOrCreate<SocialLinks>();

            return Task.FromResult(Views(
                View<EditSocialViewModel>("Social.Edit.Content", model =>
                {
                    model.FacebookUrl = socialLinks.FacebookUrl;
                    model.TwitterUrl = socialLinks.TwitterUrl;
                    model.YouTubeUrl = socialLinks.YouTubeUrl;
                    return model;
                }).Order(10)
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditSocialViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }
           
            if (updater.ModelState.IsValid)
            {

                // Store social links in generic UserData store
                var data = user.GetOrCreate<SocialLinks>();
                data.FacebookUrl = model.FacebookUrl;
                data.TwitterUrl = model.TwitterUrl;
                data.YouTubeUrl = model.YouTubeUrl;
                user.AddOrUpdate<SocialLinks>(data);

                // Update user
                await _platoUserStore.UpdateAsync(user);

            }

            return await BuildEditAsync(user, updater);

        }
        
    }
}
