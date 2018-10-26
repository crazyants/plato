using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Plato.Reactions.Models;

namespace Plato.Discuss.Reactions.ViewModels
{
    public class TopicReactionsViewModel
    {
        public IDictionary<string, IList<IReaction>> Reactions { get; set; } = 
            new ConcurrentDictionary<string, IList<IReaction>>();

    }
}
