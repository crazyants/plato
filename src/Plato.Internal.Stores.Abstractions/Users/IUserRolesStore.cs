using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IPlatoUserRoleStore<T> : IStore2<T> where T : class
    {

        Task<IEnumerable<T>> AddUserRolesAsync(int userId, IEnumerable<string> roleNames);

        Task<IEnumerable<T>> AddUserRolesAsync(int userId, IEnumerable<int> roleIds);

        Task<bool> DeleteUserRolesAsync(int userId);

        Task<IEnumerable<T>> GetUserRoles(int ustId);
        
        Task<bool> DeleteUserRole(int userId, int roleId);

    }
}