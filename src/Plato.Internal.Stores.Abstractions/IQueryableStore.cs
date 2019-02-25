using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions
{

    /// <summary>
    /// Represents a store that supports an IQueryBuilder.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryableStore<TModel> where TModel : class
    {

        IQuery<TModel> QueryAsync();

        Task<IPagedResults<TModel>> SelectAsync(params object[] args);

    }
    
}
