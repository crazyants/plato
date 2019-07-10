using System.Threading.Tasks;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Users.Badges.Services;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{

    public class BadgeViewProvider : BaseViewProvider<Badge>
    {
        
        private readonly IBadgeEntriesStore _badgeEntriesStore;

        public BadgeViewProvider(IBadgeEntriesStore badgeEntriesStore)
        {
      
            _badgeEntriesStore = badgeEntriesStore;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Badge badge, IViewProviderContext context)
        {

            var badges = await _badgeEntriesStore.SelectAsync();
            var viewModel = new BadgesIndexViewModel()
            {
                Badges = badges
            };

            return Views(View<BadgesIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                View<BadgesIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<BadgesIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
            );
            
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Badge userProfile, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Badge model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Badge model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }
}
