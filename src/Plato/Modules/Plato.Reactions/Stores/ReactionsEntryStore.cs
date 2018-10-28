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

    public class ReactionsEntryStore<TReaction> : IReactionsEntryStore<TReaction> where TReaction : class, IReactionEntry
    {
        private readonly IStringLocalizer _localizer;
        private readonly IReactionsManager<Reaction> _reactionManager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionStore;

        public ReactionsEntryStore(
            IReactionsManager<Reaction> reactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionStore,
            IStringLocalizer localizer)
        {
            _reactionManager = reactionManager;
            _entityReactionStore = entityReactionStore;
            _localizer = localizer;
        }

        #region "Implementation"

        public async Task<IDictionary<string, ReactionEntryGrouped>> GetReactionsAsync(int entityId, int entityReplyId)
        {
            
            // Get all reactions for entity
            var entityReactions = await _entityReactionStore.QueryAsync()
                .Select<EntityReactionsQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();
            
            // Build reactions
            //var output = new List<TReaction>();

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
                
                // Iterate all entity reactions
                foreach (var entityReaction in entityReactions?.Data.Where(r => r.EntityReplyId == entityReplyId))
                {
                    // Get provided reaction
                    var reaction = reactionsList.FirstOrDefault(b => b.Name.Equals(entityReaction.ReactionName, StringComparison.OrdinalIgnoreCase));
                    if (reaction != null)
                    {
                     
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
        
        public async Task<IEnumerable<GroupedReaction>> GetReactionsGroupedAsync(int entityId, int entityReplyId)
        {
            
            var output = new List<GroupedReaction>();
            foreach (var reaction in await GetReactionsAsync(entityId, entityReplyId))
            {
                int max = 15, i = 0;
                var name = reaction.Value.Reaction.Name;
                var emoji = reaction.Value.Reaction.Emoji;
                              
                var sb = new StringBuilder();
                foreach (var item in reaction.Value.Users)
                {
                    sb.Append(item.DisplayName);
                    if (i < reaction.Value.Users.Count - 1)
                    {
                        sb.Append(", ");
                    }
                    if (i >= max)
                    {
                        break;
                    }
                    i++;
                }
                sb.Append(" ")
                    .Append(_localizer["reacted with the"].Value)
                    .Append(" ")
                    .Append(name.ToLower())
                    .Append(" ")
                    .Append(_localizer["emoji"].Value);

                output.Add(new GroupedReaction()
                {
                    Emoji = reaction.Key,
                    Name = name,
                    Total = reaction.Value.Users.Count.ToPrettyInt(),
                    ToolTip = sb.ToString()
                });
            }

            return output;
        }

        #endregion
        
        #region "Private Methods"

        //async Task<IDictionary<string, GroupedByUserReaction>> GetReactionsGroupedByEmojiAsync(
        //    int entityId,
        //    int entityReplyId)
        //{
        //    var output = new ConcurrentDictionary<string, GroupedByUserReaction>();
        //    foreach (var reaction in await GetReactionsAsync(entityId, entityReplyId))
        //    {
        //        output.AddOrUpdate(reaction.Emoji,
        //            new GroupedByUserReaction(reaction),
        //            (k, v) =>
        //            {
        //                v.Users.Add(reaction.CreatedBy);
        //                return v;
        //            });
        //    }
            
        //    return output;

        //}

        #endregion

    }

}
