using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Repositories.Badges;
using Plato.Internal.Stores.Abstractions.Badges;

namespace Plato.Internal.Stores.Badges
{

    //public interface IBadgeEntryStore
    //{

    //    Task<IEnumerable<IBadgeEntry>> SelectAsync();

    //    Task<IEnumerable<IBadgeEntry>> SelectByUserIdAsync(int userId);

    //}

    //public class BadgeEntryStore : IBadgeEntryStore
    //{

    //    private readonly IAggregatedUserBadgeRepository _aggregatedUserBadgeRepository;
    //    private readonly IUserBadgeStore<UserBadge> _userBadgeStore;
    //    private readonly IBadgesManager<Badge> _badgesManager;

    //    public BadgeEntryStore(
    //        IUserBadgeStore<UserBadge> userBadgeStore,
    //        IAggregatedUserBadgeRepository aggregatedUserBadgeRepository)
    //    {
    //        _userBadgeStore = userBadgeStore;
    //        _aggregatedUserBadgeRepository = aggregatedUserBadgeRepository;
    //    }


    //    public async Task<IEnumerable<IBadgeEntry>> SelectAsync(int userId)
    //    {

    //        // Get all available badges
    //        var badges = _badgesManager.GetBadges();
    //        var userBadges = await GetUserBadges(userId, badges);

    //        return await MergeDetails(userBadges ?? badges);

    //    }

    //    public async Task<IEnumerable<IBadgeEntry>> MergeDetails(IEnumerable<IBadge> badges)
    //    {

    //        var badgesList = badges.ToList();
    //        if (badgesList.Count == 0)
    //        {
    //            return null;
    //        }
            
    //        // Get all badge details
    //        var details = await _aggregatedUserBadgeRepository.SelectAsync();
    //        var detailsList = details?.ToList();

    //        var output = new List<BadgeEntry>();
    //        foreach (var badge in badgesList)
    //        {
    //            var detail = detailsList?.FirstOrDefault(b => b.BadgeName.Equals(badge.Name, StringComparison.OrdinalIgnoreCase));
    //            output.Add(detail != null
    //                ? new BadgeEntry(badge, detail)
    //                : new BadgeEntry(badge));
    //        }
            
    //        return output;

    //    }

    //    public async Task<IEnumerable<IBadge>> GetUserBadges(int userId, IList<IBadge> badges)
    //    {

    //        if (badges == null)
    //        {
    //            return null;
    //        }

    //        var output = new List<IBadge>();
    //        var userBadges = await _userBadgeStore.QueryAsync()
    //                .Select<UserBadgeQueryParams>(q =>
    //                {
    //                    q.UserId.Equals(userId);
    //                })
    //                .OrderBy("Id", OrderBy.Asc)
    //                .ToList();

    //        if (userBadges?.Data != null)
    //        {
    //            foreach (var userBadge in userBadges.Data)
    //            {
    //                var badge = badges.FirstOrDefault(b => b.Name.Equals(userBadge.BadgeName, StringComparison.OrdinalIgnoreCase));
    //                if (badge != null)
    //                {
    //                    output.Add(badge);
    //                }
    //            }
    //        }

    //        return output;

    //    }

    //}

}
