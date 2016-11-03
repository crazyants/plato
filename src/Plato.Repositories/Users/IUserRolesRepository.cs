using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Repositories.Users
{
    public interface IUserRolesRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> InsertUserRolesAsync(int userId, IEnumerable<string> roleNames);

        Task<IEnumerable<T>> InsertUserRolesAsync(int userId, IEnumerable<int> roleIds);

        Task<IEnumerable<T>> SelectUserRoles(int userId);

        Task<bool> DeletetUserRolesAsync(int userId);

        Task<bool> DeletetUserRole(int userId, string roleName);

        Task<bool> DeletetUserRole(int userId, int roleId);
    }
}