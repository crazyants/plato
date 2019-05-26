using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;
using Plato.Notifications.Repositories;

namespace Plato.Notifications.Stores
{
    
    public class UserNotificationsStore : IUserNotificationsStore<UserNotification>
    {
        
        private readonly IUserNotificationsRepository<UserNotification> _entityMentionsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<UserNotificationsStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public UserNotificationsStore(
            IUserNotificationsRepository<UserNotification> entityMentionsRepository,
            ICacheManager cacheManager,
            ILogger<UserNotificationsStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityMentionsRepository = entityMentionsRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        public async Task<UserNotification> CreateAsync(UserNotification model)
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
            
            var result = await _entityMentionsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<UserNotification> UpdateAsync(UserNotification model)
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
            
            var result = await _entityMentionsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }
        
        public async Task<bool> DeleteAsync(UserNotification model)
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
        
        public async Task<UserNotification> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityMentionsRepository.SelectByIdAsync(id));

        }

        public IQuery<UserNotification> QueryAsync()
        {
            var query = new UserNotificationsQuery(this);
            return _dbQuery.ConfigureQuery<UserNotification>(query); ;
        }
        
        public async Task<IPagedResults<UserNotification>> SelectAsync(DbParam[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity labels for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _entityMentionsRepository.SelectAsync(dbParams);

            });
        }

        public async Task<bool> UpdateReadDateAsync(int userId, DateTimeOffset? readDate)
        {
            var success = await _entityMentionsRepository.UpdateReadDateAsync(userId, readDate);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updating ReadDate userId '{0}' to {1}",
                        userId, readDate.ToString());
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }
    }

}
