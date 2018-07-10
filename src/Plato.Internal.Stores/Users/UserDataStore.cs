using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public interface IUserDataStore<T> : IStore<T> where T : class
    {

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


        public Task<UserData> CreateAsync(UserData model)
        {
            throw new NotImplementedException();
        }

        public Task<UserData> UpdateAsync(UserData model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(UserData model)
        {
            throw new NotImplementedException();
        }

        public Task<UserData> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
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

    }
}
