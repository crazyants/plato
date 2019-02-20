using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions
{
    public interface IDecorator<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> DecorateAsync(IEnumerable<TModel> models);

        Task<TModel> DecorateAsync(TModel model);

    }

}
