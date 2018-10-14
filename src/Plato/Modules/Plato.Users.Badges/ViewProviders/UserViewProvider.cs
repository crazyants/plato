using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Badges.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<UserProfile>
    {

        private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IUserBadgeStore<UserBadge> userBadgeStore)
        {
            _platoUserStore = platoUserStore;
            _userBadgeStore = userBadgeStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile userProfile,
            IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }

            var badges = await _userBadgeStore.GetUserBadgesAsync(user.Id);
            var viewModel = new UserBadgesIndexViewModel()
            {
                User = user,
                Badges = badges
            };

            return Views(
                View<UserBadgesIndexViewModel>("Profile.Display.Sidebar", model => viewModel).Zone("sidebar").Order(2)
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
