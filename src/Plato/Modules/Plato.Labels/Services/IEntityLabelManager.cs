using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Labels.Services
{
    public interface IEntityLabelManager<TEntityLabel> where TEntityLabel : class
    {
        Task<IActivityResult<TEntityLabel>> CreateAsync(TEntityLabel model);

        Task<IActivityResult<TEntityLabel>> UpdateAsync(TEntityLabel model);

        Task<IActivityResult<TEntityLabel>> DeleteAsync(TEntityLabel model);

    }

}
