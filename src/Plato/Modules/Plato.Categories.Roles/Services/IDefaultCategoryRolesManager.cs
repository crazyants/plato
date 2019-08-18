using System.Threading.Tasks;
using Plato.Categories.Models;

namespace Plato.Categories.Roles.Services
{
    public interface IDefaultCategoryRolesManager<TCategory> where TCategory : class, ICategory
    {

        Task InstallAsync();

        Task InstallAsync(int featureId);

    }
    
}
