using System.Threading.Tasks;

namespace Plato.Questions.Categories.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
