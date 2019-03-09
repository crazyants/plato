namespace Plato.Entities.Reactions.Models
{
    
    public interface IReaction
    {

        string Category { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Emoji { get; set; }

        Sentiment Sentiment { get; set; }
        
        int Points { get; set; }
        
    }

    public enum Sentiment
    {
        Negative = -1,
        Neutral = 0,
        Positive = 1
    }


}
