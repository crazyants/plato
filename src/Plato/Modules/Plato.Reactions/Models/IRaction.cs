namespace Plato.Reactions.Models
{
    public interface IReaction
    {
        string Category { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Emoji { get; set; }

        int Points { get; set; }
    }

}
