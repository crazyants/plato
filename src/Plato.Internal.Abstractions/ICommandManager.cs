using System.Threading.Tasks;

namespace Plato.Internal.Abstractions
{
    public interface ICommandManager<TModel> where TModel : class
    {

        Task<ICommandResult<TModel>> CreateAsync(TModel model);

        Task<ICommandResult<TModel>> UpdateAsync(TModel model);

        Task<ICommandResult<TModel>> DeleteAsync(TModel model);

    }

}
