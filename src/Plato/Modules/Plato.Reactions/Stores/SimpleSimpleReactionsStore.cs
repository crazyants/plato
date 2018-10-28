using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Reactions.Stores
{

    public class SimpleSimpleReactionsStore<TReaction> : ISimpleReactionsStore<TReaction> where TReaction : class, IReactionEntry
    {

        // Maximum number of users to show within the tooltip text
        private const int Max = 15;

        private readonly IStringLocalizer _localizer;
        private readonly IReactionsManager<Reaction> _reactionManager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionStore;

        public SimpleSimpleReactionsStore(
            IReactionsManager<Reaction> reactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionStore,
            IStringLocalizer localizer)
        {
            _reactionManager = reactionManager;
            _entityReactionStore = entityReactionStore;
            _localizer = localizer;
        }
        
        public async Task<IEnumerable<SimpleReaction>> GetSimpleReactionsAsync(int entityId, int entityReplyId)
        {
            
            var output = new List<SimpleReaction>();
            foreach (var reaction in await GetReactionsAsync(entityId, entityReplyId))
            {
                // Build tooltip
                var i = 0;
                var sb = new StringBuilder();
                foreach (var item in reaction.Value.Users)
                {
                    sb.Append(item.DisplayName);
                    if (i < reaction.Value.Users.Count - 1)
                    {
                        sb.Append(", ");
                    }
                    if (i >= Max)
                    {
                        break;
                    }
                    i++;
                }
                sb.Append(" ")
                    .Append(_localizer["reacted with the"].Value)
                    .Append(" ")
                    .Append(reaction.Value.Name.ToLower())
                    .Append(" ")
                    .Append(_localizer["emoji"].Value);

                // Build simple reaction
                output.Add(new SimpleReaction()
                {
                    Emoji = reaction.Value.Emoji,
                    Name = reaction.Value.Name,
                    Total = reaction.Value.Users.Count.ToPrettyInt(),
                    ToolTip = sb.ToString()
                });
            }

            return output;
        }
        
        async Task<IDictionary<string, ReactionEntryGrouped>> GetReactionsAsync(int entityId, int entityReplyId)
        {

            // Get all reactions for entity (this includes entity replies for perf)
            var entityReactions = await _entityReactionStore.QueryAsync()
                .Select<EntityReactionsQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();

            var output = new ConcurrentDictionary<string, ReactionEntryGrouped>();
            if (entityReactions != null)
            {

                // Get provided reactions
                var reactions = _reactionManager.GetReactions();
                if (reactions == null)
                {
                    return null;
                }

                // No reactions have been provided
                var reactionsList = reactions.ToList();
                if (reactionsList.Count == 0)
                {
                    return null;
                }

                // Iterate all entity reactions matching supplied eneityId and entityReplyId
                foreach (var entityReaction in entityReactions?.Data.Where(r => r.EntityReplyId == entityReplyId))
                {

                    // Get provided reaction for entry
                    var reaction = reactionsList.FirstOrDefault(b => b.Name.Equals(entityReaction.ReactionName, StringComparison.OrdinalIgnoreCase));
                    if (reaction != null)
                    {
                        // Create a ditionary for the reaction with all
                        // users who have reacted with this reaction
                        output.AddOrUpdate(reaction.Emoji,
                            new ReactionEntryGrouped(new ReactionEntry(reaction)
                            {
                                CreatedBy = entityReaction.CreatedBy,
                                CreatedDate = entityReaction.CreatedDate
                            }),
                            (k, v) =>
                            {
                                v.Users.Add(entityReaction.CreatedBy);
                                return v;
                            });
                    }

                }

            }

            return output;

        }
        
    }

}
