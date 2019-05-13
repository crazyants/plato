using System.Threading.Tasks;

namespace Plato.Issues.Categories.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
