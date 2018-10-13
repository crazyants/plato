using System;

namespace Plato.Badges.Models
{

    public class Badge : IBadge
    {

        public string ModuleId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Threshold { get; set; }

        public int BonusPoints { get; set; }

        public bool Enabled { get; set; }

        public BadgeLevel Level { get; set; }

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
            this.Threshold = threshold;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, level, threshold)
        {
            this.BonusPoints = bonusPoints;
        }

    }

}
