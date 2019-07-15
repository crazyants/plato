using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories
{
    
    /// <summary>
    /// Represents a repository that supports creating, deleting & querying data.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IRepository<TModel> : IQueryableRepository<TModel> where TModel : class
    {

        Task<TModel> InsertUpdateAsync(TModel model);

        Task<TModel> SelectByIdAsync(int id);
        
        Task<bool> DeleteAsync(int id);

    }
    
}