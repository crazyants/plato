using System.Threading.Tasks;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Services
{
    public interface IPostManager<TModel> where TModel : class
    {

        Task<IActivityResult<TModel>> CreateAsync(TModel model);

        Task<IActivityResult<TModel>> UpdateAsync(TModel model);

        Task<IActivityResult<TModel>> DeleteAsync(int id);

    }

}
