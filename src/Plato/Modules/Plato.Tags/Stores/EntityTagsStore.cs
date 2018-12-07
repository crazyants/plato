using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Repositories;

namespace Plato.Tags.Stores
{

    public class EntityTagStore : IEntityTagStore<EntityTag>
    {

        private const string ByEntityId = "ByEntityId";
        private const string ByEntityReplyId = "ByEntityReplyId";

        private readonly IEntityTagsRepository<EntityTag> _entityLabelRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EntityTagStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityTagStore(
            IEntityTagsRepository<EntityTag> entityLabelRepository,
            ICacheManager cacheManager,
            ILogger<EntityTagStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityLabelRepository = entityLabelRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<EntityTag> CreateAsync(EntityTag model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.TagId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TagId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityLabelRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<EntityTag> UpdateAsync(EntityTag model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.TagId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TagId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityLabelRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityTag model)
        {
            var success = await _entityLabelRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity tag for entityId '{0}' and tagId {1}",
                        model.EntityId, model.TagId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityTag> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityLabelRepository.SelectByIdAsync(id));
        }

        public IQuery<EntityTag> QueryAsync()
        {
            var query = new EntityTagQuery(this);
            return _dbQuery.ConfigureQuery<EntityTag>(query); ;
        }

        public async Task<IPagedResults<EntityTag>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity labels for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _entityLabelRepository.SelectAsync(args);

            });
        }

        public async Task<IEnumerable<EntityTag>> GetByEntityId(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity tags for entity Id '{0}'.",
                        entityId);
                }

                return await _entityLabelRepository.SelectByEntityId(entityId);

            });
        }

        public async Task<IEnumerable<EntityTag>> GetByEntityReplyId(int entityReplyId)
        {

            if (entityReplyId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityReplyId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityReplyId, entityReplyId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity tags for entity reply Id '{0}'.",
                        entityReplyId);
                }

                return await _entityLabelRepository.SelectByEntityReplyId(entityReplyId);

            });
        }


        public async Task<bool> DeleteByEntityId(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var success = await _entityLabelRepository.DeleteByEntityId(entityId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all tags for entityId '{0}'",
                        entityId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<bool> DeleteByEntityIdAndTagIdId(int entityId, int tagId)
        {

            var success = await _entityLabelRepository.DeleteByEntityIdAndTagId(entityId, tagId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity Label for entityId '{0}' and tagId {1}",
                        entityId, tagId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        #endregion

    }

}
