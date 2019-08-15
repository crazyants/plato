using System;
using System.Collections.Generic;
using System.Data;
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
        
        private readonly IStarRepository<Star> _starRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogger<StarStore> _logger;

        public StarStore(
            IStarRepository<Star> starRepository,
            IDbQueryConfiguration dbQuery,
            IKeyGenerator keyGenerator,
            ICacheManager cacheManager,
            ILogger<StarStore> logger)
        {
            _starRepository = starRepository;
            _keyGenerator = keyGenerator;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

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

            var result = await _starRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
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
            
            var result = await _starRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
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
            
            var success = await _starRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted follow with EntityId {0} UserId {1}",
                        model.ThingId, model.CreatedUserId);
                }

                CancelTokens(model);

            }

            return success;
        }

        public async Task<Star> GetByIdAsync(int id)
        {
            return await _starRepository.SelectByIdAsync(id);
        }

        public IQuery<Star> QueryAsync()
        {
            var query = new StarQuery(this);
            return _dbQuery.ConfigureQuery<Star>(query); ;
        }

        public async Task<IPagedResults<Star>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting follows for key '{0}' with parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _starRepository.SelectAsync(dbParams);

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

                return await _starRepository.SelectByNameAndThingId(name, thingId);

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

                return await _starRepository.SelectByNameThingIdAndCreatedUserId(name, thingId, createdUserId);

            });
        }

        public void CancelTokens(Star model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
