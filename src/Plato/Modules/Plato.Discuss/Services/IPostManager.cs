using System.Threading.Tasks;
using Plato.Entities.Services;

namespace Plato.Discuss.Services
{
    public interface IPostManager<TModel> where TModel : class
    {

        Task<IEntityResult<TModel>> CreateAsync(TModel model);

        Task<IEntityResult<TModel>> UpdateAsync(TModel model);

        Task<IEntityResult<TModel>> DeleteAsync(int id);

    }

}
