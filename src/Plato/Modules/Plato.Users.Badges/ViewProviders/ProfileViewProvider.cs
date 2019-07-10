using System.Threading.Tasks;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Badges;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Badges.Services;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{

    public class ProfileViewProvider : BaseViewProvider<Profile>
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBadgeEntriesStore _badgeEntriesStore;

        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IUserBadgeStore<UserBadge> userBadgeStore,
            IBadgesManager<Badge> badgesManager, IBadgeEntriesStore badgeEntriesStore)
        {
            _platoUserStore = platoUserStore;
            _badgeEntriesStore = badgeEntriesStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Profile profile,
            IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(profile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }
            
            var badges = await _badgeEntriesStore.SelectByUserIdAsync(user.Id);
            var viewModel = new ProfileDisplayViewModel()
            {
                User = user,
                Badges = badges
            };

            return Views(
                View<ProfileDisplayViewModel>("Profile.Display.Content", model => viewModel).Zone("content").Order(0)
                //View<ProfileDisplayViewModel>("Profile.Display.Sidebar", model => viewModel).Zone("sidebar").Order(2)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Profile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
