using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    public class PlatoUserStore : IPlatoUserStore<User>
    {

        #region "Private Variables"
        
        private readonly ICacheManager _cacheManager;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly IUserRepository<User> _userRepository;
        private readonly ILogger<PlatoUserStore> _logger;

        #endregion

        #region "constructor"

        public PlatoUserStore(
            IDbQueryConfiguration dbQuery,
            IUserRepository<User> userRepository,
            IMemoryCache memoryCache,
            ILogger<PlatoUserStore> logger, ICacheManager cacheManager)
        {
            _dbQuery = dbQuery;
            _userRepository = userRepository;
            _logger = logger;
            _cacheManager = cacheManager;
        }

        #endregion

        #region "IPlatoUserStore"

        public async Task<User> CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            }

            var newUser = await _userRepository.InsertUpdateAsync(user);
            if (newUser != null)
            {

                // Ensure new users have an API key, update this after adding the user
                // so we can append the newly generated unique userId to the guid
                if (String.IsNullOrEmpty(newUser.ApiKey))
                {
                    newUser.ApiKey = System.Guid.NewGuid().ToString() + user.Id.ToString();
                    newUser = await UpdateAsync(user);
                }

                ClearCache(user);
            }

            return newUser;
        }

        public async Task<User> UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Id == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(user.Id));
            }

            if (String.IsNullOrEmpty(user.ApiKey))
            {
                user.ApiKey = System.Guid.NewGuid().ToString() + user.Id.ToString();
            }

            var updatedUser = await _userRepository.InsertUpdateAsync(user);
            if (updatedUser != null)
            {
                ClearCache(user);
            }

            return updatedUser;
        }
        
        public async Task<bool> DeleteAsync(User user)
        {
            
            var success = await _userRepository.DeleteAsync(user.Id);
            if (success)
            {
                ClearCache(user);
            }

            return success;

        }

        public async Task<User> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userRepository.SelectByIdAsync(id));
        }

        public async Task<User> GetByUserNameNormalizedAsync(string userNameNormalized)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userNameNormalized);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userRepository.SelectByUserNameNormalizedAsync(userNameNormalized));
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userName);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userRepository.SelectByUserNameAsync(userName));
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), email);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userRepository.SelectByEmailAsync(email));
        }

        public async Task<User> GetByApiKeyAsync(string apiKey)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), apiKey);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userRepository.SelectByApiKeyAsync(apiKey));
        }
        
        public IQuery<User> QueryAsync()
        {
            var query = new UserQuery(this);
            return _dbQuery.ConfigureQuery(query); ;
        }

    
        public async Task<IPagedResults<User>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userRepository.SelectAsync(args));
        }

        #endregion

        #region "Private Methods"

        private void ClearCache(User user)
        {
            _cacheManager.CancelTokens(this.GetType());
            _cacheManager.CancelTokens(this.GetType(), user.Id);
            _cacheManager.CancelTokens(this.GetType(), user.UserName);
            _cacheManager.CancelTokens(this.GetType(), user.Email);
            _cacheManager.CancelTokens(this.GetType(), user.NormalizedUserName);
            _cacheManager.CancelTokens(this.GetType(), user.ApiKey);
        }

        #endregion

    }
}