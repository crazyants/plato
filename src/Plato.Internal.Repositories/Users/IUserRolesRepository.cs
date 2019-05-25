using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserRolesRepository<T> : IRepository2<T> where T : class
    {

        Task<IEnumerable<T>> SelectUserRolesByUserId(int userId);

        Task<IEnumerable<T>> InsertUserRolesAsync(int userId, IEnumerable<string> roleNames);

        Task<IEnumerable<T>> InsertUserRolesAsync(int userId, IEnumerable<int> roleIds);

        Task<bool> DeleteUserRolesAsync(int userId);

        Task<bool> DeleteUserRole(int userId, int roleId);
    }
}