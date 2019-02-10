using Plato.Internal.Stores.Abstractions;

namespace Plato.Users.StopForumSpam.Stores
{
    public interface IStopForumSpamSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
