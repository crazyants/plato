using System;

namespace Plato.Internal.Models.Badges
{

    public interface IBadgeEntry
    {
        DateTimeOffset? AwardedDate { get; set; }

    }

    public class BadgeEntry : Badge, IBadgeEntry
    {
        public BadgeEntry(IBadge badge) :
            base(badge.Name,
                badge.Description,
                badge.BackgroundIconCss,
                badge.IconCss,
                badge.Level,
                badge.Threshold,
                badge.BonusPoints)
        {
        }

        public DateTimeOffset? AwardedDate { get; set; }

    }
}
