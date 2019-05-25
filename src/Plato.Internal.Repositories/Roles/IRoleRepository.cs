using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Roles
{
    public interface IRoleRepository<T> : IRepository2<T> where T : class
    {
        Task<T> SelectByNameAsync(string name);

        Task<IList<T>> SelectByUserIdAsync(int userId);

        Task<IEnumerable<T>> SelectRoles();
        
        Task<T> SelectByNormalizedNameAsync(string nameNormalized);

        
    }
}