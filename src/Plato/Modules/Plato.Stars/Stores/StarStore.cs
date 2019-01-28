using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Stores;
using Plato.Stars.Models;
using Plato.Stars.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Stars.Stores
{

    public class StarStore : IStarStore<Star>
    {
        
        private readonly IStarRepository<Star> _followRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<StarStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly IKeyGenerator _keyGenerator;

        public StarStore(
            IStarRepository<Star> followRepository,
            ICacheManager cacheManager,
            ILogger<StarStore> logger,
            IDbQueryConfiguration dbQuery,
            IKeyGenerator keyGenerator)
        {
            _followRepository = followRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
            _keyGenerator = keyGenerator;
        }

        #region "Implementation"

        public async Task<Star> CreateAsync(Star model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.ThingId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (String.IsNullOrEmpty(model.CancellationToken))
            {
                model.CancellationToken = _keyGenerator.GenerateKey(o =>
                {
                    o.MaxLength = 100;
                });
            }

            var follow = await _followRepository.InsertUpdateAsync(model);
            if (follow != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return follow;
        }

        public async Task<Star> UpdateAsync(Star model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.ThingId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (String.IsNullOrEmpty(model.CancellationToken))
            {
                model.CancellationToken = _keyGenerator.GenerateKey(o =>
                {
                    o.MaxLength = 100;
                });
            }
            
            var follow = await _followRepository.InsertUpdateAsync(model);
            if (follow != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return follow;
        }

        public async Task<bool> DeleteAsync(Star model)
        {
            
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            var success = await _followRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted follow with EntityId {0} UserId {1}",
                        model.ThingId, model.CreatedUserId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<Star> GetByIdAsync(int id)
        {
            return await _followRepository.SelectByIdAsync(id);
        }

        public IQuery<Star> QueryAsync()
        {
            var query = new StarQuery(this);
            return _dbQuery.ConfigureQuery<Star>(query); ;
        }

        public async Task<IPagedResults<Star>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting follows for key '{0}' with parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _followRepository.SelectAsync(args);

            });
        }

        public async Task<IEnumerable<Star>> SelectByNameAndThingId(string name, int thingId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), name, thingId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding followers for name {0} with thingId of {1} to cache with key {2}",
                        name, thingId, token.ToString());
                }

                return await _followRepository.SelectByNameAndThingId(name, thingId);

            });
        }

        public async Task<Star> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), name, thingId, createdUserId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding follow details for name {0} with createdUserId of {1} and thingId of {2} to cache with key {3}",
                      name,  createdUserId, thingId, token.ToString());
                }

                return await _followRepository.SelectByNameThingIdAndCreatedUserId(name, thingId, createdUserId);

            });
        }

        #endregion

    }

}
