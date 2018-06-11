using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserBannerRepository<T> : IRepository<T> where T : class
    {

        Task<T> SelectByUserIdAsync(int userId);

    }
}
