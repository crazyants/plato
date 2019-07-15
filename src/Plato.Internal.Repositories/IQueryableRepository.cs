using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories
{

    /// <summary>
    /// Represents a repository that supports an IPagedResults.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryableRepository<TModel> where TModel : class
    {
        Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams);

    }

}
