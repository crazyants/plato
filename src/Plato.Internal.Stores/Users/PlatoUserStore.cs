using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{
    public class PlatoUserStore : IPlatoUserStore<User>
    {

        #region "Private Variables"

        private readonly IUserDataItemStore<UserData> _userDataItemStore;
        private readonly IUserDataStore<UserData> _userDataStore;
        private readonly ICacheManager _cacheManager;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly IUserRepository<User> _userRepository;
        private readonly ILogger<PlatoUserStore> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        #endregion

        #region "constructor"

        public PlatoUserStore(
            IDbQueryConfiguration dbQuery,
            IUserRepository<User> userRepository,
            IMemoryCache memoryCache,
            ILogger<PlatoUserStore> logger,
            ICacheManager cacheManager, 
            ITypedModuleProvider typedModuleProvider,
            IUserDataItemStore<UserData> userDataItemStore, 
            IUserDataStore<UserData> userDataStore)
        {
            _dbQuery = dbQuery;
            _userRepository = userRepository;
            _logger = logger;
            _cacheManager = cacheManager;
            _typedModuleProvider = typedModuleProvider;
            _userDataItemStore = userDataItemStore;
            _userDataStore = userDataStore;
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
           
            // transform meta data
            user.Data = await SerializeMetaDataAsync(user);

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
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByIdAsync(id);
                return await MergeUserData(user);
            });
        }

        public async Task<User> GetByUserNameNormalizedAsync(string userNameNormalized)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userNameNormalized);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByUserNameNormalizedAsync(userNameNormalized);
                return await MergeUserData(user);
            });
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userName);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByUserNameAsync(userName);
                return await MergeUserData(user);
            });
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), email);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByEmailAsync(email);
                return await MergeUserData(user);
            });
        }

        public async Task<User> GetByApiKeyAsync(string apiKey)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), apiKey);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var user = await _userRepository.SelectByApiKeyAsync(apiKey);
                return await MergeUserData(user);
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

                var results = await _userRepository.SelectAsync(args);
                if (results != null)
                {
                    results.Data = await MergeUserData(results.Data);
                }
                return results;

            });
        }

        #endregion

        #region "Private Methods"

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
        
        async Task<IList<User>> MergeUserData(IList<User> users)
        {

            if (users == null)
            {
                return null;
            }

            // Get all user data matching supplied user ids
            var results = await _userDataStore.QueryAsync()
                .Select<UserDataQueryParams>(q => { q.UserId.IsIn(users.Select(u => u.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return users;
            }

            // Merge data into users
            return await MergeUserData(users, results.Data);

        }

        async Task<IList<User>> MergeUserData(IList<User> users, IList<UserData> data)
        {

            if (users == null || data == null)
            {
                return users;
            }

            for (var i = 0; i < users.Count; i++)
            {
                users[i].Data = data.Where(d => d.Id == users[i].Id).ToList();
                users[i] = await MergeUserData(users[i]);
            }

            return users;

        }
        
        async Task<User> MergeUserData(User user)
        {

            if (user == null)
            {
                return null;
            }

            if (user.Data == null)
            {
                return user;
            }

            foreach (var data in user.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    user.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return user;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

        void ClearCache(User user)
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