using System.Threading.Tasks;

namespace Plato.Articles.Categories.Services
{

    public interface IChannelDetailsUpdater
    {
        Task UpdateAsync(int channelId);
    }

}
