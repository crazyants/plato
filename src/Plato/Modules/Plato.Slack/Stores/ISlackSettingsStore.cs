using Plato.Internal.Stores.Abstractions;

namespace Plato.Slack.Stores
{
    public interface ISlackSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
