using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Models;
using Plato.Reactions.Models;

namespace Plato.Discuss.Reactions.ViewModels
{
    public class TopicReactionsViewModel
    {

        public IEntity Topic { get; set; }

        public IDictionary<string, IList<IReaction>> Reactions { get; set; } = 
            new ConcurrentDictionary<string, IList<IReaction>>();

    }
}
