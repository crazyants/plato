using Plato.Internal.Abstractions;

namespace Plato.Https.Models
{

    public class HttpsSettings : Serializable
    {
        public bool EnforceSsl { get; set; }

        public bool UsePermanentRedirect { get; set; }

        public int SslPort { get; set; }

    }

}
