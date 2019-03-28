using System.Threading.Tasks;
using Plato.Categories.Models;

namespace Plato.Discuss.Channels.Services
{

    public interface IChannelDetailsUpdater
    {
        Task UpdateAsync(int categoryId);
    }

}
