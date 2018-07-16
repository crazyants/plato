using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Stores;
using Plato.Follow.Models;
using Plato.Follow.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;

namespace Plato.Follow.Stores
{

    public class EntityFollowStore : IEntityFollowStore<EntityFollow>
    {
        
        private readonly IEntityFollowRepository<EntityFollow> _entityFollowRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EmailStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityFollowStore(
            IEntityFollowRepository<EntityFollow> entityFollowRepository,
            ICacheManager cacheManager,
            ILogger<EmailStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityFollowRepository = entityFollowRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<EntityFollow> CreateAsync(EntityFollow model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            if (String.IsNullOrEmpty(model.CancellationGuid))
            {
                model.CancellationGuid = System.Guid.NewGuid().ToString();
            }

            var follow = await _entityFollowRepository.InsertUpdateAsync(model);
            if (follow != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return follow;
        }

        public async Task<EntityFollow> UpdateAsync(EntityFollow model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            if (String.IsNullOrEmpty(model.CancellationGuid))
            {
                model.CancellationGuid = System.Guid.NewGuid().ToString();
            }


            var follow = await _entityFollowRepository.InsertUpdateAsync(model);
            if (follow != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return follow;
        }

        public async Task<bool> DeleteAsync(EntityFollow model)
        {
            
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            var success = await _entityFollowRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted follow with EntityId {0} UserId {1}",
                        model.EntityId, model.UserId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<EntityFollow> GetByIdAsync(int id)
        {
            return await _entityFollowRepository.SelectByIdAsync(id);
        }

        public IQuery<EntityFollow> QueryAsync()
        {
            var query = new EntityFollowQuery(this);
            return _dbQuery.ConfigureQuery<EntityFollow>(query); ;
        }

        public async Task<IPagedResults<EntityFollow>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entities for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _entityFollowRepository.SelectAsync(args);

            });
        }

        public async Task<IEnumerable<EntityFollow>> SelectEntityFollowsByEntityId(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding followers for entity {0} to cache with key {1}",
                        entityId, token.ToString());
                }

                return await _entityFollowRepository.SelectEntityFollowsByEntityId(entityId);

            });
        }

        public async Task<EntityFollow> SelectEntityFollowByUserIdAndEntityId(int userId, int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding follow details for userId {0} and entityId {1} to cache with key {2}",
                       userId, entityId, token.ToString());
                }

                return await _entityFollowRepository.SelectEntityFollowByUserIdAndEntityId(userId, entityId);

            });
        }

        #endregion

    }
}
