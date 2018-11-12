using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Mentions.Models;
using Plato.Mentions.Repositories;

namespace Plato.Mentions.Stores
{
    
    public class EntityMentionsStore : IEntityMentionsStore<EntityMention>
    {
        
        private readonly IEntityMentionsRepository<EntityMention> _entityMentionsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EntityMentionsStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityMentionsStore(
            IEntityMentionsRepository<EntityMention> entityMentionsRepository,
            ICacheManager cacheManager,
            ILogger<EntityMentionsStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityMentionsRepository = entityMentionsRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        public async Task<EntityMention> CreateAsync(EntityMention model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            if (model.EntityId <= 0 && model.EntityReplyId <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(model.EntityId)} or {nameof(model.EntityReplyId)} must be greater than zero.");
            }

            var result = await _entityMentionsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<EntityMention> UpdateAsync(EntityMention model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            if (model.EntityId <= 0 && model.EntityReplyId <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(model.EntityId)} or {nameof(model.EntityReplyId)} must be greater than zero.");
            }

            var result = await _entityMentionsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }
        
        public async Task<bool> DeleteAsync(EntityMention model)
        {
            var success = await _entityMentionsRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted mention role for userId '{0}' with id {1}",
                        model.UserId, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }
        
        public async Task<EntityMention> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityMentionsRepository.SelectByIdAsync(id));

        }

        public IQuery<EntityMention> QueryAsync()
        {
            var query = new EntityMentionsQuery(this);
            return _dbQuery.ConfigureQuery<EntityMention>(query); ;
        }
        
        public async Task<IPagedResults<EntityMention>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity labels for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _entityMentionsRepository.SelectAsync(args);

            });
        }


        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {
            var success = await _entityMentionsRepository.DeleteByEntityIdAsync(entityId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all mentions for entity Id '{0}'",
                        entityId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<bool> DeleteByEntityReplyIdAsync(int entityReplyId)
        {
            var success = await _entityMentionsRepository.DeleteByEntityReplyIdAsync(entityReplyId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all mentions for entity reply with Id '{0}'",
                        entityReplyId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

    }

}
