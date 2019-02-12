using Plato.Internal.Stores.Abstractions;

namespace Plato.StopForumSpam.Stores
{
    public interface IStopForumSpamSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
