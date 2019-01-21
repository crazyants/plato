using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public class PlatoUserStore : IPlatoUserStore<User>
    {

        #region "Private Variables"
        
        private const string ByUsername = "ByUsername";
        private const string ByUsernameNormalized = "ByUsernameNormalized";
        private const string ByEmail = "ByEmail";
        private const string ByEmailNormalized = "ByEmailNormalized";
        private const string ByResetToken = "ByResetToken";
        private const string ByConfirmationToken = "ByConfirmationToken";
        private const string ByApiKey = "ByApiKey";
        private const string PlatoBot = "PlatoBot";

        private readonly IUserDataStore<UserData> _userDataStore;
        private readonly ICacheManager _cacheManager;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly IUserRepository<User> _userRepository;
        private readonly ILogger<PlatoUserStore> _logger;
        private readonly IAliasCreator _aliasCreator;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IUserDataMerger _userDataMerger;

        #endregion

        #region "constructor"

        public PlatoUserStore(
            IDbQueryConfiguration dbQuery,
            IUserRepository<User> userRepository,
            ILogger<PlatoUserStore> logger,
            ICacheManager cacheManager, 
            IUserDataStore<UserData> userDataStore,
            IAliasCreator aliasCreator,
            IKeyGenerator keyGenerator,
            IUserDataMerger userDataMerger)
        {
            _userRepository = userRepository;
            _cacheManager = cacheManager;
            _userDataStore = userDataStore;
            _aliasCreator = aliasCreator;
            _keyGenerator = keyGenerator;
            _userDataMerger = userDataMerger;
            _dbQuery = dbQuery;
            _logger = logger;
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

            // Create alias
            user.Alias = _aliasCreator.Create(user.DisplayName);

            // transform meta data
            user.Data = await SerializeMetaDataAsync(user);
            
            var newUser = await _userRepository.InsertUpdateAsync(user);
            if (newUser != null)
            {

                // Ensure new users have an API key, update this after adding the user
                // so we can append the newly generated unique userId to the guid
                if (String.IsNullOrEmpty(newUser.ApiKey))
                {
                    newUser = await UpdateAsync(newUser);
                }

                CancelTokens(user);
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

            // Generate a default api key
            if (String.IsNullOrEmpty(user.ApiKey))
            {
                user.ApiKey = _keyGenerator.GenerateKey(o =>
                {
                    o.UniqueIdentifier = user.Id.ToString();
                });
            }

            // Update alias
            user.Alias = _aliasCreator.Create(user.DisplayName);
            
            // transform meta data
            user.Data = await SerializeMetaDataAsync(user);

            var updatedUser = await _userRepository.InsertUpdateAsync(user);
            if (updatedUser != null)
            {
                // Cancel tokens for old user
                CancelTokens(user);
                // Cancel tokens for updated user
                CancelTokens(updatedUser);
            }

            return updatedUser;
        }
        
        public async Task<bool> DeleteAsync(User user)
        {
            
            var success = await _userRepository.DeleteAsync(user.Id);
            if (success)
            {
                CancelTokens(user);
            }

            return success;

        }

        public async Task<User> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByIdAsync(id);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByUserNameNormalizedAsync(string userNameNormalized)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByUsernameNormalized, userNameNormalized);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByUserNameNormalizedAsync(userNameNormalized);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByUsername, userName);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByUserNameAsync(userName);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEmail, email);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByEmailAsync(email);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByEmailNormalizedAsync(string emailNormalized)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEmailNormalized, emailNormalized);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByEmailNormalizedAsync(emailNormalized);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByResetToken(string resetToken)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByResetToken, resetToken);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByResetTokenAsync(resetToken);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByConfirmationToken(string confirmationToken)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByConfirmationToken, confirmationToken);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByConfirmationTokenAsync(confirmationToken);
                return await _userDataMerger.MergeAsync(user);
            });
        }

        public async Task<User> GetByApiKeyAsync(string apiKey)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByApiKey, apiKey);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByApiKeyAsync(apiKey);
                return await _userDataMerger.MergeAsync(user);
            });
        }
        
        public IQuery<User> QueryAsync()
        {
            var query = new UserQuery(this);
            return _dbQuery.ConfigureQuery(query); ;
        }
        
        public async Task<IPagedResults<User>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting users for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }
                
                var results = await _userRepository.SelectAsync(args);
                if (results != null)
                {
                    results.Data = await _userDataMerger.MergeAsync(results.Data);
                }
                return results;
            });
        }

        public async Task<User> GetPlatoBotAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), PlatoBot);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await QueryAsync().Take(1)
                    .Select<UserQueryParams>(q => q.UserType.Equals(UserType.Bot))
                    .ToList();
                return results?.Data?[0];
            });
        }

        #endregion

        #region "Private Methods"

        /// <summary>
        /// Serialize all meta data on the user object into JSON for storage within UserData table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<IEnumerable<UserData>> SerializeMetaDataAsync(User user)
        {

            // Get all existing user data
            var data = await _userDataStore.GetByUserIdAsync(user.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<UserData>();
            
            // Iterate all meta data on the supplied user object,
            // check if a key already exists, if so update existing key 
            var output = new List<UserData>();
            foreach (var item in user.MetaData)
            {
                var key = item.Key.FullName;
                var userData = dataList.FirstOrDefault(d => d.Key == key);
                if (userData != null)
                {
                    userData.Value = item.Value.Serialize();
                }
                else
                {
                    userData = new UserData()
                    {
                        Key = key,
                        Value = item.Value.Serialize()
                    };
                }

                output.Add(userData);
            }

            return output;

        }
        
        void CancelTokens(User user)
        {

            // Expire user cache
            _cacheManager.CancelTokens(this.GetType());
            _cacheManager.CancelTokens(this.GetType(), user.Id);
            _cacheManager.CancelTokens(this.GetType(), ByUsernameNormalized, user.NormalizedUserName);
            _cacheManager.CancelTokens(this.GetType(), ByUsername, user.UserName);
            _cacheManager.CancelTokens(this.GetType(), ByEmailNormalized, user.NormalizedEmail);
            _cacheManager.CancelTokens(this.GetType(), ByEmail, user.Email);
            _cacheManager.CancelTokens(this.GetType(), ByResetToken, user.ResetToken);
            _cacheManager.CancelTokens(this.GetType(), ByConfirmationToken, user.ConfirmationToken);
            _cacheManager.CancelTokens(this.GetType(), ByApiKey, user.ApiKey);

            // Expire user data cache
            _cacheManager.CancelTokens(typeof(UserDataStore));
            _cacheManager.CancelTokens(typeof(UserDataStore), "ByUser", user.Id);

        }

        #endregion

    }

}