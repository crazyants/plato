using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IUserDataMerger
    {
        Task<IList<User>> MergeAsync(IList<User> users);

        Task<User> MergeAsync(User user);

    }

}
