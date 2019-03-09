using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Reactions.Stores
{

    public class SimpleReactionsStore : ISimpleReactionsStore
    {

        // Maximum number of users to show within the tooltip text
        private const int ByMax = 15;

        private readonly IStringLocalizer _localizer;
        private readonly IReactionsManager<Reaction> _reactionManager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionStore;

        public SimpleReactionsStore(
            IStringLocalizer localizer,
            IReactionsManager<Reaction> reactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionStore)
        {
            _localizer = localizer;
            _reactionManager = reactionManager;
            _entityReactionStore = entityReactionStore;
        }
        
        public async Task<IEnumerable<SimpleReaction>> GetSimpleReactionsAsync(int entityId, int entityReplyId)
        {

            var text1 = _localizer["reacted with the"].Value;
            var text2 = _localizer["emoji"].Value;

            var output = new List<SimpleReaction>();
            foreach (var reaction in await GetReactionsAsync(entityId, entityReplyId))
            {
                // Build information tooltip
                var i = 0;
                var sb = new StringBuilder();
                foreach (var user in reaction.Value.Users)
                {
                    sb.Append(user.DisplayName);
                    if (i < reaction.Value.Users.Count - 1)
                    {
                        sb.Append(", ");
                    }
                    if (i >= ByMax)
                    {
                        break;
                    }
                    i++;
                }
                sb.Append(" ")
                    .Append(text1)
                    .Append(" ")
                    .Append(reaction.Value.Name.ToLower())
                    .Append(" ")
                    .Append(text2);

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
                        // Create a dictionary with all users who have reacted with this reaction
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
