namespace Plato.Badges.Models
{

    public class Badge : IBadge
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public int Threshold { get; set; }

        public int BonusPoints { get; set; }

        public bool Enabled { get; set; }

        public BadgeLevel Level { get; set; }

    }

}
