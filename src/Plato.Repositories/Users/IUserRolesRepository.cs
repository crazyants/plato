using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Repositories.Users
{
    public interface IUserRolesRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> InsertUserRoles(int userId, IEnumerable<string> roleNames);

        Task<IEnumerable<T>> InsertUserRoles(int userId, IEnumerable<int> roleIds);

        bool DeletetUserRoles(int userId);

        bool DeletetUserRole(int userId, string roleName);

        bool DeletetUserRole(int userId, int roleId);

    }

}
