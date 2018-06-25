using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewComponents;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoRoleStore _platoRoleStore;

        public UserViewProvider(
            UserManager<User> userManager,
            IPlatoRoleStore platoRoleStore)
        {
            _userManager = userManager;
            _platoRoleStore = platoRoleStore;
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
            var selectedRoles = await _platoRoleStore.GetRoleNamesByUserIdAsync(user.Id);

            return Views(
                View<EditRoleViewModel>("UserRoles.Edit.Content", model => { return model; }).Order(2),
                View("SelectRoles",
                    new
                    {
                        selectedRoles = selectedRoles,
                        htmlName = ""
                    }).Order(2)
            );

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
