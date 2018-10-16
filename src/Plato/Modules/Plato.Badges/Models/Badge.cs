using System;

namespace Plato.Badges.Models
{

    public class Badge : IBadge
    {

        // Globally multiply the default thredhold and bounus points for all badges
        // Default value should be set to 1 increase the accomodate requirements
        private static readonly int ThresholdMultiplier = 1;
        private static readonly int BonusPointsMultiplier = 1;

        public string Category { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string BackgroundIconCss { get; set; } = "fas fa-badge";

        public string IconCss { get; set; } = "fal fa-star";
        
        public int Threshold { get; set; }

        public int BonusPoints { get; set; }

        public bool Enabled { get; set; }

        public BadgeLevel Level { get; set; }

        public Action<IBadgeAwarderContext> Awarder { get; set; }
        
        public DateTimeOffset? AwardedDate { get; set; }


        public Badge(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Badge(string name,string description) : this(name)
        {
            this.Description = description;
        }


        public Badge(
            string name,
            string description,
            string backgroundIconCss) : this(name, description)
        {
            this.BackgroundIconCss = backgroundIconCss;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss) : this(name, description, backgroundIconCss)
        {
            this.IconCss = iconCss;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level) : this(name, description, backgroundIconCss, iconCss)
        {
            this.Level = level;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level) : this(name, description)
        {
            this.Level = level;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold) : this(name, description, backgroundIconCss, iconCss, level)
        {
            this.Threshold = threshold * ThresholdMultiplier;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold) : this(name, description, level)
        {
            this.Threshold = threshold * ThresholdMultiplier;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, backgroundIconCss, iconCss, level, threshold)
        {
            this.BonusPoints = bonusPoints * BonusPointsMultiplier;
        }
        
        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, level, threshold)
        {
            this.BonusPoints = bonusPoints * BonusPointsMultiplier;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            Action<IBadgeAwarderContext> awarder) : this(name, description, backgroundIconCss, iconCss, level)
        {
            this.Awarder = awarder;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            Action<IBadgeAwarderContext> awarder) : this(name, description, level)
        {
            this.Awarder = awarder;
        }

        public Badge(
            string name,
            string description,
            string iconCss,
            BadgeLevel level,
            Action<IBadgeAwarderContext> awarder) : this(name, description, "fas fa-badge", iconCss, level)
        {
            this.Awarder = awarder;
        }


        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            Action<IBadgeAwarderContext> awarder) : this(name, description, level, threshold)
        {
            this.Awarder = awarder;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints,
            Action<IBadgeAwarderContext> awarder) : this(name, description, backgroundIconCss, iconCss, level, threshold, bonusPoints)
        {
            this.Awarder = awarder;
        }

        public Badge(
            string name,
            string description,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints,
            Action<IBadgeAwarderContext> awarder) : this(name, description, "fas fa-badge", iconCss, level, threshold, bonusPoints)
        {
            this.Awarder = awarder;
        }


        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            int bonusPoints,
            Action<IBadgeAwarderContext> awarder) : this(name, description, level, threshold, bonusPoints)
        {
            this.Awarder = awarder;
        }

    }

}
