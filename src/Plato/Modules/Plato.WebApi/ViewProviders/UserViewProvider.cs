using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;

using Plato.WebApi.ViewModels;

namespace Plato.WebApi.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public UserViewProvider(
            UserManager<User> userManager,
            IPlatoUserStore<User> platoUserStore)
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

        public override Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {

            // Don't adapt the view when creating new users
            if (user.Id == 0)
            {
                return Task.FromResult(default(IViewProviderResult));
            }
            
            return Task.FromResult(Views(
                View<EditUserViewModel>("User.Edit.Content", model =>
                {
                    model.ApiKey = user.ApiKey;
                    return model;
                }).Order(10)
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }
            
            if (updater.ModelState.IsValid)
            {

                user.ApiKey = model.ApiKey;

                await _platoUserStore.UpdateAsync(user);
                
            }
            
            return await BuildEditAsync(user, updater);

        }
        
    }
}
