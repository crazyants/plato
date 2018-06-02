using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Repositories.Users;
using Plato.Stores.Roles;

namespace Plato.Stores.Users
{
    public class UserRolesStore : IPlatoUserRoleStore<UserRole>
    {
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IUserRolesRepository<UserRole> _userRolesRepository;

        public UserRolesStore(
            IPlatoRoleStore platoRoleStore,
            IUserRolesRepository<UserRole> userRolesRepository)
        {
            _platoRoleStore = platoRoleStore;
            _userRolesRepository = userRolesRepository;
        }

        public async Task<IEnumerable<UserRole>> AddtUserRolesAsync(int userId, IEnumerable<string> roleNames)
        {
            return await _userRolesRepository.InsertUserRolesAsync(userId, roleNames);
        }

        public async Task<IEnumerable<UserRole>> AddUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            return await _userRolesRepository.InsertUserRolesAsync(userId, roleIds);
        }

        public Task<UserRole> CreateAsync(UserRole model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(UserRole model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletetUserRole(int userId, int roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletetUserRole(int userId, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletetUserRolesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserRole>> GetUserRoles(int ustId)
        {
            throw new NotImplementedException();
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> UpdateAsync(UserRole model)
        {
            throw new NotImplementedException();
        }
    }
}