using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Badges.Stores;
using Plato.Internal.Data.Abstractions;
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
        private readonly IBadgesManager<Badge> _badgesManager;

        public UserBadgeViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            IUserBadgeStore<UserBadge> userBadgeStore,
            IBadgesManager<Badge> badgesManager)
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
            
            var viewModel = new UserBadgesIndexViewModel()
            {
                User = user,
                Badges = await GetBadgesAsync(user)
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

        async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(int userId)
        {
            var results = await _userBadgeStore.QueryAsync()
                .Select<UserBadgeQueryParams>(q =>
                {
                    q.UserId.Equals(userId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();
            if (results?.Data != null)
            {
                return results.Data;
            }
            return new List<UserBadge>();
        }

       async Task<IEnumerable<Badge>> GetBadgesAsync(User user)
        {

            if (user == null)
            {
                return null;
            }

            var badges = _badgesManager.GetBadges();
            if (badges == null)
            {
                return null;
            }

            var badgesList = badges.ToList();
            if (badgesList.Count == 0)
            {
                return null;
            }

            var output = new List<Badge>();
            foreach (var userBadge in await GetUserBadgesAsync(user.Id))
            {
                var badge = badgesList.FirstOrDefault(b => b.Name.Equals(userBadge.BadgeName, StringComparison.OrdinalIgnoreCase));
                if (badge != null)
                {
                    output.Add(badge);
                }
            }
           
            return output;

        }

    }
}
