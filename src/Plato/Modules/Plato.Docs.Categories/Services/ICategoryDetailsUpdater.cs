using System.Threading.Tasks;

namespace Plato.Docs.Categories.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
