using System.Threading.Tasks;

namespace Plato.Discuss.Channels.Services
{

    public interface IChannelDetailsUpdater
    {
        Task UpdateAsync(int channelId);
    }

}
