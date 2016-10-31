using System.Threading.Tasks;

namespace Plato.Repositories.Roles
{
    public interface IRoleRepository<T> : IRepository<T> where T : class
    {
        Task<T> SelectByNameAsync(string name);
    }
}