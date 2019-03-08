using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Reactions.Models;

namespace Plato.Entities.Reactions.ViewModels
{
    public class ReactionMenuViewModel
    {

        public IEntity Entity { get; set; }

        public IEntityReply Reply { get; set; }

        public IEnumerable<IReaction> Reactions { get; set; }

    }
}
