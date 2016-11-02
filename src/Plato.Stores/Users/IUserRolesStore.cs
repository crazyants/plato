using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Stores;
using Plato.Models.Users;

namespace Plato.Stores.Users
{
    public interface IPlatoRoleStore<T> : IStore<T> where T : class
    {
   
        Task<IEnumerable<T>> AddtUserRolesAsync(int userId, IEnumerable<string> roleNames);

        Task<IEnumerable<T>> AddUserRolesAsync(int userId, IEnumerable<int> roleIds);

        Task<bool> DeletetUserRolesAsync(int userId);

        Task<bool> DeletetUserRole(int userId, string roleName);

        Task<bool> DeletetUserRole(int userId, int roleId);
        

    }
}
