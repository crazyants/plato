using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Repositories.Badges;
using Plato.Internal.Stores.Abstractions.Badges;
using Plato.Internal.Stores.Badges;

namespace Plato.Users.Badges.Services
{
    
    public class BadgeEntriesStore : IBadgeEntriesStore
    {

        private readonly IBadgeDetailsRepository _badgeDetailsRepository;
        private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
        private readonly IBadgesManager<Badge> _badgesManager;

        public BadgeEntriesStore(
            IBadgeDetailsRepository badgeDetailsRepository,
            IUserBadgeStore<UserBadge> userBadgeStore,
            IBadgesManager<Badge> badgesManager)
        {
            _badgeDetailsRepository = badgeDetailsRepository;
            _userBadgeStore = userBadgeStore;
            _badgesManager = badgesManager;
        }

        public async Task<IEnumerable<IBadgeEntry>> SelectAsync()
        {
            var details = await _badgeDetailsRepository.SelectAsync();
            return MergeDetails(_badgesManager.GetBadges(), details);
        }

        public async Task<IEnumerable<IBadgeEntry>> SelectByUserIdAsync(int userId)
        {

            // Get all available badges
            var badges = _badgesManager.GetBadges();
            var userBadges = await GetUserBadges(userId, badges.ToList());
            var details = await _badgeDetailsRepository.SelectByUserIdAsync(userId);
            return MergeDetails(userBadges ?? badges, details);

        }

        // --------------

        IEnumerable<IBadgeEntry> MergeDetails(
            IEnumerable<IBadge> badges,
            IEnumerable<IBadgeDetails> details)
        {

            if (badges == null)
            {
                return null;
            }

            var badgesList = badges.ToList();
            if (badgesList.Count == 0)
            {
                return null;
            }
           
            var detailsList = details?.ToList();
            var output = new List<BadgeEntry>();
            foreach (var badge in badgesList)
            {
                var detail = detailsList?.FirstOrDefault(b => b.BadgeName.Equals(badge.Name, StringComparison.OrdinalIgnoreCase));
                output.Add(detail != null
                    ? new BadgeEntry(badge, detail)
                    : new BadgeEntry(badge));
            }
            
            return output;

        }

        async Task<IEnumerable<IBadge>> GetUserBadges(int userId, IList<Badge> badges)
        {

            if (badges == null)
            {
                return null;
            }

            var output = new List<IBadge>();
            var userBadges = await _userBadgeStore.QueryAsync()
                    .Select<UserBadgeQueryParams>(q =>
                    {
                        q.UserId.Equals(userId);
                    })
                    .OrderBy("Id", OrderBy.Asc)
                    .ToList();

            if (userBadges?.Data != null)
            {
                foreach (var userBadge in userBadges.Data)
                {
                    var badge = badges.FirstOrDefault(b => b.Name.Equals(userBadge.BadgeName, StringComparison.OrdinalIgnoreCase));
                    if (badge != null)
                    {
                        output.Add(badge);
                    }
                }
            }

            return output;

        }

    }

}
