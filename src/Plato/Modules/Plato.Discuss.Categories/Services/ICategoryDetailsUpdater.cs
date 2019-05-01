using System.Threading.Tasks;

namespace Plato.Discuss.Categories.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
