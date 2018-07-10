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
        private readonly IUserDetailsStore<UserDetail> _userDetailsStore;

        public UserViewProvider(
            UserManager<User> userManager, IPlatoUserStore<User> platoUserStore, IUserDetailsStore<UserDetail> userDetailsStore)
        {
            _userManager = userManager;
            _platoUserStore = platoUserStore;
            _userDetailsStore = userDetailsStore;
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
            var details = await _userDetailsStore.GetAsync(user.Id);

            return Views(
                View<EditUserViewModel>("User.Edit.Content", model =>
                {
                    model.ApiKey = details.ApiKey;
                    return model;
                }).Order(10)
            );

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

                var details = await _userDetailsStore.GetAsync(user.Id);
                details.ApiKey = model.ApiKey;
                await _userDetailsStore.UpdateAsync(user.Id, details);
                
            }
            
            return await BuildEditAsync(user, updater);

        }
        
    }
}
