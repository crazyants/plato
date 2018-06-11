using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public interface IPlatoUserRoleStore<T> : IStore<T> where T : class
    {
        Task<IEnumerable<T>> AddtUserRolesAsync(int userId, IEnumerable<string> roleNames);

        Task<IEnumerable<T>> AddUserRolesAsync(int userId, IEnumerable<int> roleIds);

        Task<bool> DeletetUserRolesAsync(int userId);

        Task<IEnumerable<T>> GetUserRoles(int ustId);

        Task<bool> DeletetUserRole(int userId, string roleName);

        Task<bool> DeletetUserRole(int userId, int roleId);
    }
}