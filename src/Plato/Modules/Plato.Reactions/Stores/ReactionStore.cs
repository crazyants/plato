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

    public class ReactionsStore<TReaction> : IReactionsStore<TReaction> where TReaction : class, IReaction
    {
        private readonly IStringLocalizer _localizer;
        private readonly IReactionsManager<TReaction> _reactionManager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionStore;

        public ReactionsStore(
            IReactionsManager<TReaction> reactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionStore,
            IStringLocalizer localizer)
        {
            _reactionManager = reactionManager;
            _entityReactionStore = entityReactionStore;
            _localizer = localizer;
        }

        #region "Implementation"

        public async Task<IEnumerable<TReaction>> GetEntityReactionsAsync(int entityId, int entityReplyId)
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

            // Get all reactions for entity
            var entityReactions = await _entityReactionStore.QueryAsync()
                .Select<EntityReactionsQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();
            
            // Build reactions
            var output = new List<TReaction>();
            if (entityReactions != null)
            {
                foreach (var entityReaction in entityReactions?.Data.Where(r => r.EntityReplyId == entityReplyId))
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
        
        public async Task<IEnumerable<GroupedReaction>> GetEntityReactionsGroupedAsync(int entityId, int entityReplyId)
        {

            var reactions = await GetReactionsGroupedByEmojiAsync(entityId, entityReplyId);
            var output = new List<GroupedReaction>();
            foreach (var reaction in reactions)
            {
                int max = 15, i = 0;
                var name = "";
                var sb = new StringBuilder();
                foreach (var item in reaction.Value)
                {
                    name = item.Name;
                    sb.Append(item.CreatedBy.DisplayName);
                    if (i < reaction.Value.Count - 1)
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
                    Total = reaction.Value.Count.ToPrettyInt(),
                    ToolTip = sb.ToString()
                });
            }

            return output;
        }

        public Task<IEnumerable<GroupedReaction>> GetEntityReplyReactionsGroupedAsync(int entityReplyId)
        {
            throw new NotImplementedException();
        }

        #endregion
        
        #region "Private Methods"

        async Task<IDictionary<string, IList<IReaction>>> GetReactionsGroupedByEmojiAsync(int entityId, int entityReplyId)
        {

            var output = new ConcurrentDictionary<string, IList<IReaction>>();
            var reactions = await GetEntityReactionsAsync(entityId, entityReplyId);
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
        
        #endregion

    }

}
