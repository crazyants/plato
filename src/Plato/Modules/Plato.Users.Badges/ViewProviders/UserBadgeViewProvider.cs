using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Badges.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{
    public class UserBadgeViewProvider : BaseViewProvider<UserBadge>
    {

        private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
   
        public UserBadgeViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            IUserBadgeStore<UserBadge> userBadgeStore)
        {
            _platoUserStore = platoUserStore;
            _userBadgeStore = userBadgeStore;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserBadge userProfile,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(UserBadge badge, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(badge.UserId);
            if (user == null)
            {
                return await BuildIndexAsync(badge, context);
            }

            var badges = await _userBadgeStore.GetUserBadgesAsync(user.Id);
            var viewModel = new UserBadgesIndexViewModel()
            {
                User = user,
                Badges = badges
            };
            
            return Views(
                View<UserBadgesIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                View<UserBadgesIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<UserBadgesIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(UserBadge model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserBadge model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
