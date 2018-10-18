using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewComponents
{
    public class BadgeListViewComponent : ViewComponent
    {

        private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
        private readonly IBadgesManager<Badge> _badgeManager;

        public BadgeListViewComponent(
            IBadgesManager<Badge> badgeManager,
            IUserBadgeStore<UserBadge> userBadgeStore)
        {
            _badgeManager = badgeManager;
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

            var badges = options.UserId > 0
                ? await _userBadgeStore.GetUserBadgesAsync(options.UserId)
                : _badgeManager.GetBadges();

            return new BadgesIndexViewModel()
            {
                Badges = badges
            };

        }

    }

}
