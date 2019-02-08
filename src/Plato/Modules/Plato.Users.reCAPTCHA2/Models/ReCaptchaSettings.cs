using Plato.Internal.Abstractions;

namespace Plato.Users.reCAPTCHA2.Models
{
    public class ReCaptchaSettings : Serializable
    {
        public string SiteKey { get; set; }

        public string Secret { get; set; }

    }
}
