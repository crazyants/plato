using System.Collections.Generic;
using Plato.Internal.Models.Users;

namespace Plato.Reactions.Models
{
    public class ReactionEntryGrouped
    {

        public ReactionEntryGrouped(IReactionEntry reaction)
        {
            Reaction = reaction;
            this.Users.Add(reaction.CreatedBy);
        }

        public IReaction Reaction { get; }

        public IList<ISimpleUser> Users { get; } = new List<ISimpleUser>();

    }
}
