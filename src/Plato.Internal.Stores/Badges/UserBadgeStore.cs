using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Repositories.Badges;
using Plato.Internal.Stores.Abstractions.Badges;

namespace Plato.Internal.Stores.Badges
{
    
    public class UserBadgeStore : IUserBadgeStore<UserBadge>
    {
        
        private readonly IUserBadgeRepository<UserBadge> _userBadgeRepository;
        private readonly ILogger<UserBadgeStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
 
        public UserBadgeStore(
            IUserBadgeRepository<UserBadge> userBadgeRepository,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            ILogger<UserBadgeStore> logger)
        {
            _userBadgeRepository = userBadgeRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        public async Task<UserBadge> CreateAsync(UserBadge model)
        {
            
            var result = await _userBadgeRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<UserBadge> UpdateAsync(UserBadge model)
        {
            
            var result = await _userBadgeRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(UserBadge model)
        {

            var success = await _userBadgeRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted label '{0}' with id {1}",
                        model.BadgeName, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<UserBadge> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userBadgeRepository.SelectByIdAsync(id));
        }

        public IQuery<UserBadge> QueryAsync()
        {
            var query = new UserBadgeQuery(this);
            return _dbQuery.ConfigureQuery<UserBadge>(query); ;
        }

        public async Task<IPagedResults<UserBadge>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting user badges for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(a => a.Value));
                }

                return await _userBadgeRepository.SelectAsync(dbParams);

            });

        }
        
        public void CancelTokens(UserBadge model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
