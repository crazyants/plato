using System;
using Plato.Internal.Models.Users;

namespace Plato.Reactions.Models
{
    public class ReactionEntry : Reaction, IReactionEntry
    {

        public ReactionEntry(IReaction reaction)
        {
            this.Name = reaction.Name;
            this.Description = reaction.Description;
            this.Emoji = reaction.Emoji;
            this.Sentiment = reaction.Sentiment;
            this.Points = reaction.Points;
        }

        public ISimpleUser CreatedBy { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

    }
}
