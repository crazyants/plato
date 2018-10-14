using System;

namespace Plato.Badges.Models
{

    public class Badge : IBadge
    {

        // Globally multiply the default thredhold and bounus points for all badges
        private static readonly int ThresholdMultiplier = 1;
        private static readonly int BonusPointsMultiplier = 1;

        public string ModuleId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Threshold { get; set; }

        public int BonusPoints { get; set; }

        public bool Enabled { get; set; }

        public BadgeLevel Level { get; set; }

        public Action<AwarderContext> Awarder { get; set; }

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
            BadgeLevel level) : this(name, description)
        {
            this.Level = level;
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
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, level, threshold)
        {
            this.BonusPoints = bonusPoints * BonusPointsMultiplier;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            Action<AwarderContext> awarder) : this(name, description, level)
        {
            this.Awarder = awarder;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            Action<AwarderContext> awarder) : this(name, description, level, threshold)
        {
            this.Awarder = awarder;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            int bonusPoints,
            Action<AwarderContext> awarder) : this(name, description, level, threshold, bonusPoints)
        {
            this.Awarder = awarder;
        }

    }

}
