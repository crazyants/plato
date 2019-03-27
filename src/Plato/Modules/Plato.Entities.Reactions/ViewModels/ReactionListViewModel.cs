using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Entities.Reactions.Models;
using Plato.Internal.Security.Abstractions;

namespace Plato.Entities.Reactions.ViewModels
{
    public class ReactionListViewModel
    {

        public IEntity Entity { get; set; }

        public IEntityReply Reply { get; set; }

        public IEnumerable<SimpleReaction> Reactions { get; set; } 

        public IPermission Permission { get; set; }

    }

}
