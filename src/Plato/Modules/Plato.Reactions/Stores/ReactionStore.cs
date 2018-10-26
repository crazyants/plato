using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Reactions.Stores
{

    public class ReactionsStore<TReaction> : IReactionsStore<TReaction> where TReaction : class, IReaction
    {

        private readonly IReactionsManager<TReaction> _reactionManager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionStore;

        public ReactionsStore(
            IReactionsManager<TReaction> reactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionStore)
        {
            _reactionManager = reactionManager;
            _entityReactionStore = entityReactionStore;
        }

        public async Task<IEnumerable<TReaction>> GetEntityReactionsAsync(int entityId)
        {
            var reactions = _reactionManager.GetReactions();
            if (reactions == null)
            {
                return null;
            }

            var reactionsList = reactions.ToList();
            if (reactionsList.Count == 0)
            {
                return null;
            }

            var entityReactions = await _entityReactionStore.QueryAsync()
                .Select<EntityReactionsQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();

            var output = new List<TReaction>();
            if (entityReactions != null)
            {
                foreach (var entityReaction in entityReactions.Data)
                {
                    var reaction = reactionsList.FirstOrDefault(b => b.Name.Equals(entityReaction.ReactionName, StringComparison.OrdinalIgnoreCase));
                    if (reaction != null)
                    {
                        reaction.CreatedBy = entityReaction.CreatedBy;
                        reaction.CreatedDate = entityReaction.CreatedDate;
                        output.Add(reaction);
                    }
                }
            }

            return output;

        }
        
        public Task<IEnumerable<TReaction>> GetEntityReplyReactionsAsync(int entityReplyId)
        {
            throw new NotImplementedException();
        }

        public async Task<IDictionary<string, IList<IReaction>>> GetEntityReactionsGroupedByEmojiAsync(int entityId)
        {

            var output = new ConcurrentDictionary<string, IList<IReaction>>();
            var reactions = await GetEntityReactionsAsync(entityId);
            if (reactions != null)
            {
                foreach (var reaction in reactions)
                {
                    output.AddOrUpdate(reaction.Emoji, new List<IReaction>()
                    {
                        reaction
                    }, (k, v) =>
                    {
                        v.Add(reaction);
                        return v;
                    });
                }

            }

            return output;

        }
        
    }

}
