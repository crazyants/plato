using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.Badges.Services;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewComponents
{
    public class BadgeListViewComponent : ViewComponent
    {

        private readonly IBadgeEntriesStore _badgeEntriesStore;
       
        public BadgeListViewComponent(IBadgeEntriesStore badgeEntriesStore)
        {
            _badgeEntriesStore = badgeEntriesStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            BadgesIndexOptions options)
        {

            if (options == null)
            {
                options = new BadgesIndexOptions();
            }
            
            return View(await GetViewModel(options));

        }

        async Task<BadgesIndexViewModel> GetViewModel(BadgesIndexOptions options)
        {

            var entries = options.UserId > 0
                ? await _badgeEntriesStore.SelectByUserIdAsync(options.UserId)
                : await _badgeEntriesStore.SelectAsync();

            return new BadgesIndexViewModel()
            {
                Badges = options.UserId > 0
                    ? entries.OrderByDescending(b => b.Details.LastAwardedDate)
                    : entries.OrderBy(b => b.Level)
            };

        }

    }

}
