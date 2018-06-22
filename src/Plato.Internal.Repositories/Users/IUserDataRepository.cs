using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserDataRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectDataByUserId(int userId);

    }
}
