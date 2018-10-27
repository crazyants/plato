using System.Threading.Tasks;

namespace Plato.Internal.Abstractions
{
    public interface ICommandManager<TModel> where TModel : class
    {

        Task<IActivityResult<TModel>> CreateAsync(TModel model);

        Task<IActivityResult<TModel>> UpdateAsync(TModel model);

        Task<IActivityResult<TModel>> DeleteAsync(TModel model);

    }

}
