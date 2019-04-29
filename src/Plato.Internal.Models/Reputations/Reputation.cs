using System;

namespace Plato.Internal.Models.Reputations
{
    public class Reputation : IReputation
    {

        // Globally multiply the points for all reputations
        public static readonly int PointsMultiplier = 0;

        public string Name { get; set; }
        
        public int Points { get; set; }

        public string Category { get; set; }

        public string ModuleId { get; set; }

        public DateTimeOffset? AwardedDate { get; set; }
        
        protected Reputation(string name)
        {
            this.Name = name;
        }
        
        public Reputation(
            string name,
            int points) : this(name)
        {
            this.Points = PointsMultiplier > 0
                ? points * PointsMultiplier
                : points;
        }
        
    }

}
