using System;
using Plato.Internal.Models.Users;

namespace Plato.Reactions.Models
{

    public interface IReactionEntry : IReaction
    {
        ISimpleUser CreatedBy { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

    }

}
