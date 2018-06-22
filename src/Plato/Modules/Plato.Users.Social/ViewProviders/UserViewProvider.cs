using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Social.Models;
using Plato.Users.Social.Services;
using Plato.Users.Social.ViewModels;

namespace Plato.Users.Social.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
 
        private readonly ISocialLinksStore _socialLinksStore;

        public UserViewProvider(
            UserManager<User> userManager,
            ISocialLinksStore socialLinksStore)
        {

            _userManager = userManager;
            _socialLinksStore = socialLinksStore;
           
            
        }


        public override async Task<IViewProviderResult> BuildDisplayAsync(User user, IUpdateModel updater)
        {
            return null;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {
            return null;
        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {

            var socialLinks = await _socialLinksStore.GetAsync(user.Id);


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
                
                var result = await _socialLinksStore.UpdateAsync(user.Id, new SocialLinks()
                {
                    FacebookUrl = model.FacebookUrl,
                    TwitterUrl  = model.TwitterUrl,
                    YouTubeUrl = model.YouTubeUrl
                });
                
            }
            
            return await BuildEditAsync(user, updater);

        }
        
    }
}
