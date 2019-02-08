using Plato.Internal.Stores.Abstractions;

namespace Plato.Users.reCAPTCHA2.Stores
{
    public interface IReCaptchaSettingsStore<T> : ISettingsStore<T> where T : class
    {
    }

}
