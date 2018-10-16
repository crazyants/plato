namespace Plato.Reputations.Models
{
    public interface IReputation
    {

        string Name { get; set; }

        string Description { get; set; }

        int Points { get; set; }

        string Category { get; set; }

    }

}
