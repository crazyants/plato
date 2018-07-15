using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Services
{

    public interface IEntityManager<TEntity> where TEntity : class
    {

        event EntityEvents<TEntity>.Handler Creating;
        event EntityEvents<TEntity>.Handler Created;
        event EntityEvents<TEntity>.Handler Updating;
        event EntityEvents<TEntity>.Handler Updated;
        event EntityEvents<TEntity>.Handler Deleting;
        event EntityEvents<TEntity>.Handler Deleted;

        Task<IActivityResult<TEntity>> CreateAsync(TEntity model);

        Task<IActivityResult<TEntity>> UpdateAsync(TEntity model);

        Task<IActivityResult<TEntity>> DeleteAsync(TEntity model);

    }

}
