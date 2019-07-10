using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{
    public class UserBadgeViewProvider : BaseViewProvider<UserBadge>
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public UserBadgeViewProvider(IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
      
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserBadge model, IViewProviderContext context)
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

        public override Task<IViewProviderResult> BuildEditAsync(UserBadge badge, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserBadge badge, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
