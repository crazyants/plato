using System.Threading.Tasks;

namespace Plato.Discuss.Channels.Services
{

    public interface ICategoryDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
