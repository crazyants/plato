using System.Threading.Tasks;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Badges;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{
    public class UserBadgeViewProvider : BaseViewProvider<UserBadge>
    {

        private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBadgesManager<Badge> _badgesManager;

        public UserBadgeViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            IUserBadgeStore<UserBadge> userBadgeStore, IBadgesManager<Badge> badgesManager)
        {
            _platoUserStore = platoUserStore;
            _userBadgeStore = userBadgeStore;
            _badgesManager = badgesManager;
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

            var availableBadges = _badgesManager.GetBadges();
            var badges = await _userBadgeStore.GetUserBadgesAsync(user.Id, availableBadges);
            var viewModel = new UserBadgesIndexViewModel()
            {
                User = user,
                BadgesIndexViewModel = new BadgesIndexViewModel()
                {
                    Options = new BadgesIndexOptions()
                    {
                        UserId = user.Id
                    }
                }
            };
            
            return Views(
                View<UserBadgesIndexViewModel>("Profile.Index.Header", model => viewModel).Zone("header"),
                View<UserBadgesIndexViewModel>("Profile.Index.Tools", model => viewModel).Zone("tools"),
                View<UserBadgesIndexViewModel>("Profile.Index.Content", model => viewModel).Zone("content")
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
