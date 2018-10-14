using System;
using System.Threading.Tasks;
using Plato.Badges.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Badges.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<UserProfile>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile userProfile, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }

            //var viewModel = new UserDisplayViewModel()
            //{
            //    User = user
            //};
            var viewModel = new UserBadgesViewModel();

            return Views(
                    View<UserBadgesViewModel>("Profile.Display.Sidebar", model => viewModel).Zone("sidebar").Order(2)
                );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
