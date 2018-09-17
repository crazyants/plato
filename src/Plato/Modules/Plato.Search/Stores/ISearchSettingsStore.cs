using Plato.Internal.Stores.Abstractions;

namespace Plato.Search.Stores
{
    public interface ISearchSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
