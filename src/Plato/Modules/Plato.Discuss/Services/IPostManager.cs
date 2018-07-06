using System.Threading.Tasks;
using Plato.Entities.Services;

namespace Plato.Discuss.Services
{
    public interface IPostManager<in TModel> where TModel : class
    {

        Task<IEntityResult> CreateAsync(TModel model);

        Task<IEntityResult> UpdateAsync(TModel model);

        Task<IEntityResult> DeleteAsync(int id);

    }

}
