using Plato.Internal.Stores.Abstractions;

namespace Plato.Facebook.Stores
{
    public interface IFacebookSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
