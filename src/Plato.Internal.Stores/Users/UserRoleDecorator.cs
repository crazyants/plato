using System;
using System.Collections.Generic;
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

        public Task<IEnumerable<User>> DecorateAsync(IEnumerable<User> users)
        {

            throw new NotImplementedException();

        }

        public Task<User> DecorateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
