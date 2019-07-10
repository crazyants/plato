using System;
using Plato.Internal.Models.Reputations;

namespace Plato.Internal.Models.Badges
{

    public class Badge : IBadge
    {
        private string _title;

        private string _description;

        // Globally multiply the default threshold and bonus points for all badges
        // Default value should be set to 1 increase the accomodate requirements
        public static readonly int ThresholdMultiplier = 0;
        public static readonly int PointsMultiplier = 0;

        public string Category { get; set; }

        public string Name { get; set; }

        public string Title
        {
            get => _title?.Replace("{threshold}", this.Threshold.ToString());
            set => _title = value;
        }

        public string Description
        {
            get => _description?.Replace("{threshold}", this.Threshold.ToString());
            set => _description = value;
        }

        public string BackgroundIconCss { get; set; } = "fas fa-badge";

        public string IconCss { get; set; } = "fal fa-star";
        
        public int Threshold { get; set; }

        public int BonusPoints { get; set; }

        public bool Enabled { get; set; }

        public BadgeLevel Level { get; set; }

        public IReputation GetReputation()
        {
            return new Reputation(this.Name,  this.BonusPoints);
        }
        
        public Badge()
        {
        }

        public Badge(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name)); 
        }

        public Badge(string name, string title) : this(name)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title)); 
        }
        
        public Badge(string name, string title, string description) : this(name, title)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description)); ;
        }
        
        public Badge(
            string name,
            string title,
            string description,
            string backgroundIconCss) : this(name, title, description)
        {
            BackgroundIconCss = backgroundIconCss;
        }

        public Badge(
            string name,
            string title,
            string description,
            string backgroundIconCss,
            string iconCss) : this(name, title, description, backgroundIconCss)
        {
            IconCss = iconCss;
        }

        public Badge(
            string name,
            string title,
            string description,
            string iconCss,
            BadgeLevel level) : this(name, title, description, "fas fa-badge", iconCss)
        {
            Level = level;
        }
        public Badge(
            string name,
            string title,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level) : this(name, title, description, backgroundIconCss, iconCss)
        {
            Level = level;
        }

        public Badge(
            string name,
            string title,
            string description,
            BadgeLevel level) : this(name, title, description)
        {
            Level = level;
        }

        public Badge(
            string name,
            string title,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold) : this(name, title, description, backgroundIconCss, iconCss, level)
        {
            Threshold = ThresholdMultiplier > 0
                ? threshold * ThresholdMultiplier
                : threshold;
        }

        public Badge(
            string name,
            string title,
            string description,
            string iconCss,
            BadgeLevel level,
            int threshold) : this(name, title, description, "fas fa-badge", iconCss, level)
        {
            Threshold = Badge.ThresholdMultiplier > 0
                ? threshold * ThresholdMultiplier
                : threshold;
        }

        public Badge(
            string name,
            string title,
            string description,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, title, description, "fas fa-badge", iconCss, level, threshold)
        {
            BonusPoints = PointsMultiplier > 0 
                ? bonusPoints * PointsMultiplier
                : bonusPoints;
        }

        public Badge(
            string name,
            string title,
            string description,
            BadgeLevel level,
            int threshold) : this(name, title, description, level)
        {
            Threshold = ThresholdMultiplier > 0 
                ? threshold * ThresholdMultiplier
                : threshold;
        }

        public Badge(
            string name,
            string title,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, title, description, backgroundIconCss, iconCss, level, threshold)
        {
            BonusPoints = Badge.PointsMultiplier > 0
                ? bonusPoints * PointsMultiplier
                : bonusPoints;
        }
        
        public Badge(
            string name,
            string title,
            string description,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, title, description, level, threshold)
        {
            BonusPoints = PointsMultiplier > 0
                ? bonusPoints * PointsMultiplier
                : bonusPoints;
        }
        
    }

}
