using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

using Plato.WebApi.ViewModels;

namespace Plato.WebApi.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public UserViewProvider(IPlatoUserStore<User> platoUserStore)
        {
             _platoUserStore = platoUserStore;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
        {

            // Don't adapt the view when creating new users
            if (user.Id == 0)
            {
                return Task.FromResult(default(IViewProviderResult));
            }
            
            return Task.FromResult(Views(
                View<EditUserViewModel>("User.Edit.Content", model =>
                {
                    model.Id = user.Id;
                    model.ApiKey = user.ApiKey;
                    return model;
                }).Order(int.MaxValue - 50)
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {

            var model = new EditUserViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, context);
            }
            
            if (context.Updater.ModelState.IsValid)
            {
                user.ApiKey = model.ApiKey;
                await _platoUserStore.UpdateAsync(user);
            }
            
            return await BuildEditAsync(user, context);

        }
        
    }
}
