using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Reactions.Models;

namespace Plato.Discuss.Reactions.ViewModels
{
    public class ReactMenuViewModel
    {

        public IEntity Topic { get; set; }

        public IEnumerable<IReaction> Reactions { get; set; }

    }
}
