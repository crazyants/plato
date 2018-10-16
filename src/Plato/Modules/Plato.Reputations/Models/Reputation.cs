using System;

namespace Plato.Reputations.Models
{
    public class Reputation : IReputation
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public int Points { get; set; }

        public string Category { get; set; }

        public Action<IReputationAwarderContext> Awarder { get; set; }

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
            this.Points = points;
        }

        public Reputation(
            string name,
            string description,
            int points,
            Action<IReputationAwarderContext> awarder) : this(name, description, points)
        {
            this.Awarder = awarder;
        }

    }

}
