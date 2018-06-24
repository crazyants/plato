using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        
        public UserViewProvider(
            UserManager<User> userManager)
        {
            _userManager = userManager;
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
            return Task.FromResult(Views(
                View<EditRoleViewModel>("UserRoles.Edit.Content", model => { return model; }).Order(2)
            ));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditRoleViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }

            //model.FacebookUrl = model.UserName?.Trim();

            if (updater.ModelState.IsValid)
            {

                //var result = await _socialLinksStore.UpdateAsync(user.Id, new SocialLinks()
                //{
                //    FacebookUrl = model.FacebookUrl,
                //    TwitterUrl = model.TwitterUrl,
                //    YouTubeUrl = model.YouTubeUrl
                //});

            }

            return await BuildEditAsync(user, updater);

        }

    }
}
