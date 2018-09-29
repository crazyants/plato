using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Labels.Stores
{

    public class EntityLabelStore : IEntityLabelStore<EntityLabel>
    {

        private const string ByEntityId = "ByEntityId";
        
        private readonly IEntityLabelRepository<EntityLabel> _entityLabelRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<LabelRoleStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityLabelStore(
            IEntityLabelRepository<EntityLabel> entityLabelRepository,
            ICacheManager cacheManager,
            ILogger<LabelRoleStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityLabelRepository = entityLabelRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }
        
        #region "Implementation"

        public async Task<EntityLabel> CreateAsync(EntityLabel model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.LabelId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.LabelId));
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

        public async Task<EntityLabel> UpdateAsync(EntityLabel model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.LabelId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.LabelId));
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

        public async Task<bool> DeleteAsync(EntityLabel model)
        {
            var success = await _entityLabelRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label role for Label '{0}' with id {1}",
                        model.LabelId, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityLabel> GetByIdAsync(int id)
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityLabelRepository.SelectByIdAsync(id));

        }

        public IQuery<EntityLabel> QueryAsync()
        {
            var query = new EntityLabelQuery(this);
            return _dbQuery.ConfigureQuery<EntityLabel>(query); ;
        }

        public async Task<IPagedResults<EntityLabel>> SelectAsync(params object[] args)
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

        public async Task<IEnumerable<EntityLabel>> GetByEntityId(int entityId)
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
                    _logger.LogInformation("Selecting entity labels for entity Id '{0}'.",
                        entityId);
                }

                return await _entityLabelRepository.SelectByEntityId(entityId);

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
                    _logger.LogInformation("Deleted all Label roles for Label '{0}'",
                        entityId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<bool> DeleteByEntityIdAndLabelId(int entityId, int labelId)
        {

            var success = await _entityLabelRepository.DeleteByEntityIdAndLabelId(entityId, labelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity Label for entity Id '{0}' and LabelId {1}",
                        entityId, entityId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        #endregion

    }
}
