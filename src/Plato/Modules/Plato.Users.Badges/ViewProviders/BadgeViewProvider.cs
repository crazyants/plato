using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{

    public class BadgeViewProvider : BaseViewProvider<Badge>
    {

        private readonly IBadgesManager<Badge> _badgeManager;
   
        public BadgeViewProvider(
            IBadgesManager<Badge> badgeManager)
        {
            _badgeManager = badgeManager;
        }
        public override Task<IViewProviderResult> BuildIndexAsync(Badge badge, IViewProviderContext context)
        {

            var viewModel = new BadgesIndexViewModel()
            {
                Badges = _badgeManager.GetBadges()
            };

            return Task.FromResult(
                Views(View<BadgesIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                View<BadgesIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<BadgesIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
            ));
            
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
