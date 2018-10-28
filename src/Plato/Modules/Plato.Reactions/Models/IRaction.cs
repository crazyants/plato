using System;
using Plato.Internal.Models.Users;

namespace Plato.Reactions.Models
{

    public interface IReactionEntry : IReaction
    {
        ISimpleUser CreatedBy { get; set; }
        
        DateTimeOffset? CreatedDate { get; set; }

    }

    public interface IReaction
    {

        string Category { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Emoji { get; set; }

        Sentiment Sentiment { get; set; }

        int Points { get; set; }

  

    }

}
