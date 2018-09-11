using System.Threading.Tasks;

namespace Plato.Internal.Localization.Abstractions
{
    public interface ILocaleWatcher
    {
        Task WatchForChanges();

    }

}
