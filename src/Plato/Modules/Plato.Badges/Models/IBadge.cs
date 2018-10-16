using System;

namespace Plato.Badges.Models
{
    public interface IBadge
    {

        string Category { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string BackgroundIconCss { get; set; }

        string IconCss { get; set; }

        int Threshold { get; set; }

        int BonusPoints { get; set; }

        bool Enabled { get; set; }

        BadgeLevel Level { get; set; }

        Action<IBadgeAwarderContext> Awarder { get; set; }

        DateTimeOffset? AwardedDate { get; set; }

    }

    public enum BadgeLevel
    {
        Gold,
        Silver,
        Bronze
    }


}
