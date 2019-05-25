using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.History.Models;
using Plato.Entities.History.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.History.Stores
{

    public class EntityHistoryStore : IEntityHistoryStore<EntityHistory>
    {
        
        private readonly IEntityHistoryRepository<EntityHistory> _entityHistoryRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EntityHistoryStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
   
        public EntityHistoryStore(
            IEntityHistoryRepository<EntityHistory> entityHistoryRepository,
            ICacheManager cacheManager,
            ILogger<EntityHistoryStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityHistoryRepository = entityHistoryRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<EntityHistory> CreateAsync(EntityHistory model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Id should be 0 for inserts
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            // We always need an EntityId
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
           
            var follow = await _entityHistoryRepository.InsertUpdateAsync(model);
            if (follow != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return follow;
        }

        public async Task<EntityHistory> UpdateAsync(EntityHistory model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We need an Id for updates
            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // We always need an EntityId
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var follow = await _entityHistoryRepository.InsertUpdateAsync(model);
            if (follow != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return follow;
        }

        public async Task<bool> DeleteAsync(EntityHistory model)
        {
            
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            var success = await _entityHistoryRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity history for entity id {0}, entity reply id {1} from user id {1}",
                        model.EntityId, model.EntityReplyId, model.CreatedUserId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityHistory> GetByIdAsync(int id)
        {
            return await _entityHistoryRepository.SelectByIdAsync(id);
        }

        public IQuery<EntityHistory> QueryAsync()
        {
            var query = new EntityHistoryQuery(this);
            return _dbQuery.ConfigureQuery<EntityHistory>(query); ;
        }

        public async Task<IPagedResults<EntityHistory>> SelectAsync(DbParam[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity history for key '{0}' with parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _entityHistoryRepository.SelectAsync(dbParams);

            });
        }
        
        #endregion

    }

}
