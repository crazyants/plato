using System.Threading.Tasks;

namespace Plato.Articles.Categories.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
