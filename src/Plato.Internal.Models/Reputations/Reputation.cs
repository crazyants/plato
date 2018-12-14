using System;

namespace Plato.Internal.Models.Reputations
{
    public class Reputation : IReputation
    {

        // Globally multiply the points for all reputations
        public static readonly int PointsMultiplier = 0;

        public string Name { get; set; }

        public string Description { get; set; }

        public int Points { get; set; }

        public string Category { get; set; }

        public DateTimeOffset? AwardedDate { get; set; }
        
        protected Reputation(string name)
        {
            this.Name = name;
        }

        protected Reputation(
            string name,
            string description) : this(name)
        {
            this.Description = description;
        }

        public Reputation(
            string name,
            string description,
            int points) : this(name, description)
        {
            this.Points = PointsMultiplier > 0
                ? points * PointsMultiplier
                : points;
        }
        
    }

}
