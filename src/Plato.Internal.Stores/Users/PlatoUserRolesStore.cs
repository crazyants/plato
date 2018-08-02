using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Roles;

namespace Plato.Internal.Stores.Users
{
    public class PlatoUserRolesStore : IPlatoUserRoleStore<UserRole>
    {

        private const string  Key = "UserRoles";

        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IUserRolesRepository<UserRole> _userRolesRepository;
        private readonly ILogger<PlatoUserRolesStore> _logger;
        private readonly ICacheManager _cacheManager;

        public PlatoUserRolesStore(
            IPlatoRoleStore platoRoleStore,
            IUserRolesRepository<UserRole> userRolesRepository,
            ILogger<PlatoUserRolesStore> logger,
            ICacheManager cacheManager)
        {
            _platoRoleStore = platoRoleStore;
            _userRolesRepository = userRolesRepository;
            _logger = logger;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<UserRole>> AddtUserRolesAsync(int userId, IEnumerable<string> roleNames)
        {
            var result = await _userRolesRepository.InsertUserRolesAsync(userId, roleNames);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userId);
            }

            return result;

        }

        public async Task<IEnumerable<UserRole>> AddUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            var result = await _userRolesRepository.InsertUserRolesAsync(userId, roleIds);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userId);
            }

            return result;

        }

        public async Task<UserRole> CreateAsync(UserRole model)
        {

            var userRole = await _userRolesRepository.InsertUpdateAsync(model);
            if (userRole != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userRole.UserId);
            }

            return userRole;

        }
        
        public async Task<UserRole> UpdateAsync(UserRole model)
        {
            var userRole = await _userRolesRepository.InsertUpdateAsync(model);
            if (userRole != null)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userRole.UserId);
            }

            return userRole;
        }

        public async Task<bool> DeleteAsync(UserRole model)
        {

            // Ensure the entry exists
            var userRole = await GetByIdAsync(model.Id);
            if (userRole == null)
            {
                return false;
            }

            // Delete & expire caches
            var success = await _userRolesRepository.DeleteAsync(model.Id);
            if (success)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userRole.UserId);
            }
            return success;

           

        }

        public async Task<bool> DeletetUserRole(int userId, int roleId)
        {
            var success = await _userRolesRepository.DeletetUserRole(userId, roleId);
            if (success)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userId);
            }

            return success;
        }
        
        public async Task<bool> DeletetUserRolesAsync(int userId)
        {
            var success = await _userRolesRepository.DeletetUserRolesAsync(userId);
            if (success)
            {
                _cacheManager.CancelTokens(this.GetType());
                // Expire GetRolesByUserIdAsync in PlatoRoleStore
                _cacheManager.CancelTokens(typeof(PlatoRoleStore), "ByUser", userId);
            }

            return success;

        }

        public async Task<UserRole> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _userRolesRepository.SelectByIdAsync(id));
        }

        public async Task<IEnumerable<UserRole>> GetUserRoles(int userId)
        {
            return await _userRolesRepository.SelectUserRolesByUserId(userId);
        }
        
        public IQuery<UserRole> QueryAsync()
        {
            //var query = new EntityReplyQuery(this);
            //return _dbQuery.ConfigureQuery<EntityReply>(query); ;
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserRole>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        private string GetCacheKey(int userId)
        {
            return $"{Key}_{userId.ToString()}";
        }
        
    }
}