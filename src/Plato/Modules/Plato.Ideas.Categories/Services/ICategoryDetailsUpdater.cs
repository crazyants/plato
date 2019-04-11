using System.Threading.Tasks;

namespace Plato.Ideas.Categories.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
