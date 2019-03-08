using System;
using Plato.Internal.Models.Users;

namespace Plato.Reactions.Models
{
    public class ReactionEntry : Reaction, IReactionEntry
    {

        public ReactionEntry(IReaction reaction)
        {
            Name = reaction.Name;
            Description = reaction.Description;
            Emoji = reaction.Emoji;
            Sentiment = reaction.Sentiment;
            Points = reaction.Points;
        }

        public ISimpleUser CreatedBy { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

    }

}
