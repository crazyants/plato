using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Reactions.Models;

namespace Plato.Discuss.Reactions.ViewModels
{
    public class ReactionListViewModel
    {

        public IEntity Topic { get; set; }

        public IEntityReply Reply { get; set; }

        public IEnumerable<SimpleReaction> Reactions { get; set; } 

    }

}
