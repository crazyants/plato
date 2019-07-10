using System;

namespace Plato.Internal.Models.Badges
{

    public interface IBadgeEntry : IBadge
    {
        IBadgeDetails Details { get; set; }

    }

    public class BadgeEntry : Badge, IBadgeEntry
    {

        public IBadgeDetails Details { get; set; } = new BadgeDetails();

        public BadgeEntry(IBadge badge) :
            base(badge.Name,
                badge.Title,
                badge.Description,
                badge.BackgroundIconCss,
                badge.IconCss,
                badge.Level,
                badge.Threshold,
                badge.BonusPoints)
        {
        }

        public BadgeEntry(IBadge badge, IBadgeDetails details) : this(badge)
        {
            Details = details;
        }

    }
}
