using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Reactions.Models;
using Plato.Reactions.Repositories;
using Plato.Reactions.Services;

namespace Plato.Reactions.Stores
{
    
    public class EntityReactionsStore : IEntityReactionsStore<EntityReaction>
    {

        private readonly IEntityReactionsRepository<EntityReaction> _entityReactionRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EntityReactionsStore> _logger;
        private readonly IReactionsManager<Reaction> _reactionManager;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityReactionsStore(
            IEntityReactionsRepository<EntityReaction> entityReactionRepository,
            ICacheManager cacheManager,
            ILogger<EntityReactionsStore> logger,
            IDbQueryConfiguration dbQuery,
            IReactionsManager<Reaction> reactionManager)
        {
            _entityReactionRepository = entityReactionRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
            _reactionManager = reactionManager;
        }

        #region "Implementation"

        public async Task<EntityReaction> CreateAsync(EntityReaction model)
        {
            var result = await _entityReactionRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<EntityReaction> UpdateAsync(EntityReaction model)
        {
            var result = await _entityReactionRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityReaction model)
        {
            var success = await _entityReactionRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted reaction '{0}' with id {1}",
                        model.ReactionName, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityReaction> GetByIdAsync(int id)
        {
            return await _entityReactionRepository.SelectByIdAsync(id);
        }

        public IQuery<EntityReaction> QueryAsync()
        {
            var query = new EntityReactionsQuery(this);
            return _dbQuery.ConfigureQuery<EntityReaction>(query); ;
        }

        public async Task<IPagedResults<EntityReaction>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting reactions for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _entityReactionRepository.SelectAsync(args);

            });
        }

        public async Task<IEnumerable<IReaction>> GetEntityReactionsAsync(int entityId)
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

            var entityReactions = await QueryAsync()
                .Select<EntityReactionsQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();

            var output = new List<Reaction>();
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

        public async Task<IEnumerable<EntityReaction>> SelectEntityReacotinsByEntityId(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding reactions for entity {0} to cache with key {1}",
                        entityId, token.ToString());
                }

                return await _entityReactionRepository.SelectEntityReactionsByEntityId(entityId);

            });
        }

        public async Task<IEnumerable<EntityReaction>> SelectEntityReactionsByUserIdAndEntityId(int userId, int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding reaction for userId {0} and entityId {1} to cache with key {2}",
                        userId, entityId, token.ToString());
                }

                return await _entityReactionRepository.SelectEntityReactionsByUserIdAndEntityId(userId, entityId);

            });
        }

        #endregion


    }
}
