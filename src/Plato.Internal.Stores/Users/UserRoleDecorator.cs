using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
 using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{

    public interface IUserRoleDecorator : IDecorator<User>
    {

    }

    public class UserRoleDecorator : IUserRoleDecorator
    {

        private readonly IPlatoUserRoleStore<UserRole> _userRoleStore;

        public UserRoleDecorator(IPlatoUserRoleStore<UserRole> userRoleStore)
        {
            _userRoleStore = userRoleStore;
        }

        public async Task<IEnumerable<User>> DecorateAsync(IEnumerable<User> users)
        {

            // Get all user data matching supplied users
            var roles = await _userRoleStore.QueryAsync()
                .Select<UserRoleQueryParams>(q =>
                {
                    q.UserId.IsIn(users.Select(u => u.Id).ToArray());
                }).ToList();

            if (users == null || roles?.Data == null)
            {
                return null;
            }

            var output = users.ToList();
            foreach (var user in output)
            {
                user.UserRoles = roles.Data
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.Role)
                    .ToList();
                user.RoleNames = user.UserRoles.Select(r => r.Name).ToList();
            }

            return output;

        }

        public async Task<User> DecorateAsync(User user)
        {
            // Get all user data matching supplied users
            var roles = await _userRoleStore.QueryAsync()
                .Select<UserRoleQueryParams>(q => { q.UserId.Equals(user.Id); })
                .ToList();

            if (user == null || roles?.Data == null)
            {
                return null;
            }
            
            user.UserRoles = roles.Data.Select(ur => ur.Role).ToList();
            user.RoleNames = user.UserRoles.Select(r => r.Name).ToList();
            
            return user;

        }
        
    }
}
