using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Stores.Abstractions.Badges;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewComponents
{
    public class BadgeListViewComponent : ViewComponent
    {

        private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
        private readonly IBadgesManager<Badge> _badgesManager;
   

        public BadgeListViewComponent(
            IBadgesManager<Badge> badgesManager,
            IUserBadgeStore<UserBadge> userBadgeStore)
        {
            _badgesManager = badgesManager;
            _userBadgeStore = userBadgeStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            BadgesIndexOptions options)
        {

            if (options == null)
            {
                options = new BadgesIndexOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<BadgesIndexViewModel> GetViewModel(
            BadgesIndexOptions options)
        {
            var availableBadges = _badgesManager.GetBadges();
            var badges = options.UserId > 0
                ? await _userBadgeStore.GetUserBadgesAsync(options.UserId, availableBadges)
                : _badgesManager.GetBadges();

            return new BadgesIndexViewModel()
            {
                Badges = badges
            };

        }

    }

}
