using System.Threading.Tasks;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Services
{
    public interface IPostManager<TEntity> where TEntity : class
    {

        Task<IActivityResult<TEntity>> CreateAsync(TEntity model);

        Task<IActivityResult<TEntity>> UpdateAsync(TEntity model);

        Task<IActivityResult<TEntity>> DeleteAsync(TEntity model);

    }

}
