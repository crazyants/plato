using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Badges.Repositories;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Badges.Stores
{
    
    public class UserBadgeStore : IUserBadgeStore<UserBadge>
    {

        private readonly IUserBadgeRepository<UserBadge> _userBadgeRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<UserBadgeStore> _logger;

        public UserBadgeStore(
            IUserBadgeRepository<UserBadge> userBadgeRepository,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            ILogger<UserBadgeStore> logger)
        {
            _userBadgeRepository = userBadgeRepository;
            _dbQuery = dbQuery;
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<UserBadge> CreateAsync(UserBadge model)
        {
            
            var newUserBadge = await _userBadgeRepository.InsertUpdateAsync(model);
            if (newUserBadge != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added new user badge with id {1}",
                        newUserBadge.Id);
                }
                
                _cacheManager.CancelTokens(this.GetType());

            }

            return newUserBadge;

        }

        public async Task<UserBadge> UpdateAsync(UserBadge model)
        {
            
            var updatedUserBadge = await _userBadgeRepository.InsertUpdateAsync(model);
            if (updatedUserBadge != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated existing user badge with id {1}",
                        updatedUserBadge.Id);
                }

                _cacheManager.CancelTokens(this.GetType());
            }

            return updatedUserBadge;

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

                _cacheManager.CancelTokens(this.GetType());

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

        public async Task<IPagedResults<UserBadge>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting user badges for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _userBadgeRepository.SelectAsync(args);

            });
        }

    }

}
