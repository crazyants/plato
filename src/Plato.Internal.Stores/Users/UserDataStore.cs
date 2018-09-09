using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public interface IUserDataStore<T> : IStore<T> where T : class
    {

        Task<T> GetByKeyAndUserIdAsync(string key, int userId);

        Task<IEnumerable<T>> GetByUserIdAsync(int userId);

    }

    public class UserDataStore : IUserDataStore<UserData>
    {
        private readonly ICacheManager _cacheManager;
        private readonly IUserDataRepository<UserData> _userDataRepository;
        private readonly ILogger<UserDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;


        public UserDataStore(
            ICacheManager cacheManager,
            IUserDataRepository<UserData> userDataRepository,
            ILogger<UserDataStore> logger,
            IDbQueryConfiguration dbQuery,
            ITypedModuleProvider typedModuleProvider)
        {
            _cacheManager = cacheManager;
            _userDataRepository = userDataRepository;
            _logger = logger;
            _dbQuery = dbQuery;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public async Task<UserData> CreateAsync(UserData model)
        {
            var result =  await _userDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;

        }

        public async Task<UserData> UpdateAsync(UserData model)
        {
            var result = await _userDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;

        }

        public async Task<bool> DeleteAsync(UserData model)
        {
            var success = await _userDataRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted user data with key '{0}' for user id {1}",
                        model.Key, model.UserId);
                }

                _cacheManager.CancelTokens(this.GetType());

            }

            return success;
        }

        public async Task<UserData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userDataRepository.SelectByIdAsync(id));
        }

        public IQuery<UserData> QueryAsync()
        {
            var query = new UserDataQuery(this);
            return _dbQuery.ConfigureQuery<UserData>(query); ;
        }

        public async Task<IPagedResults<UserData>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userDataRepository.SelectAsync(args));
        }

        public async Task<UserData> GetByKeyAndUserIdAsync(string key, int userId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), key, userId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userDataRepository.SelectByKeyAndUserIdAsync(key, userId));
            
        }

        public async Task<IEnumerable<UserData>> GetByUserIdAsync(int userId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), "ByUser", userId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userDataRepository.SelectByUserIdAsync(userId));
        }
    }
}
